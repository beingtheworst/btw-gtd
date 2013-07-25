using System;
using System.Diagnostics;
using Gtd.Client.Models;
using Gtd.CoreDomain;
using Gtd.CoreDomain.AppServices.ClientProfile;
using Gtd.CoreDomain.AppServices.TrustedSystem;

namespace Gtd.Client.Controllers
{
    /// <summary>
    /// Part of the App's infrastructure that subscribes to all messages that go through
    /// our in-memory message queue and determines if/how to handle each one.
    /// Based on the current App state, and the type of message it is currently processing,
    /// the AppController decides how the specific message should be handled at this point in time.
    /// AppController reads system and domain messages that we have defined from the in-memory queue
    /// and decides things like if it should invoke some code to update an Aggregate itself,
    /// pass the message through to the in-memory UI message bus for the UI controls to react to it,
    /// discard the current message based on current state, etc.
    /// </summary>
    
    // TODO: Hey Rinat, Terminolgy Concept count question...
    // This class is VERY similar to what "Projection(events)/ApplicationService(Cmd)" do for us in A+ES.
    // How are they the same? How are they different? Why do we need 3 different concepts/implementations?
    
    public sealed class AppController : IHandle<Message>
    {
        readonly IPublisher _uiBus;
        readonly IEventStore _eventStore;
        QueuedHandler _queue;

        readonly FiniteStateMachine<AppState> _finiteStateMachine;

        public AppController(IPublisher uiBus, IEventStore eventStore)
        {
            _uiBus = uiBus;
            _eventStore = eventStore;

            _finiteStateMachine = CreateStateMachine();
        }

        // default Fsm's intial state to "Loading"
        AppState _appState = AppState.Loading;

        #region What's a Finite State Machine (Fsm)?
        // It's just a clean way to express logic in the form of
        // "When we are in this state, and something happens, do this set of stuff. 
        // If in this other state and something happens, then do this other set of stuff."
        // This will likely help us when we implement a disconnected-client that we need to sync with server.
        // We can have a "Synchronizing" state for example, and we may handle messages differently in that state.
        #endregion
        FiniteStateMachine<AppState> CreateStateMachine()
        {
            return new FsmBuilder<AppState>()
                .InAllStates()

                    .When<UI.AddStuffWizardCompleted>().Do(PutStuffInInbox)
                    .When<UI.DefineActionWizardCompleted>().Do(AddActionToProject)
                    .When<UI.TrashStuffClicked>().Do(TrashStuff)
                    .When<UI.DefineNewProjectWizardCompleted>().Do(DefineProject)
                    .When<UI.MoveStuffToProjectClicked>().Do(MoveStuffToProject)
                    .When<UI.CompleteActionClicked>().Do(CompleteAction)
                    .When<UI.ChangeActionOutcome>().Do(ChangeOutcome)
                    .When<Dumb.ClientModelLoaded>().Do(LoadInbox)

                .InState(AppState.Loading)
                    
                    
                    .When<AppInit>().Do(InitApplication)
                    .When<Event>().Do(PassThroughEvent)
                    
                .WhenOther().Do(_uiBus.Publish)
                .Build(() => (int) _appState);
        }

        void LoadInbox(Dumb.ClientModelLoaded e)
        {
            _uiBus.Publish(e);
            _queue.Enqueue(new UI.DisplayInbox());
        }

        void AddActionToProject(UI.DefineActionWizardCompleted e)
        {
            _uiBus.Publish(e);
            var newGuid = Guid.NewGuid();
            UpdateDomain(agg => agg.DefineAction(new RequestId(newGuid), e.ProjectId, e.Outcome, new RealTimeProvider()));
        }

        TrustedSystemId _currentSystem;

        void InitApplication(AppInit obj)
        {
            _uiBus.Publish(obj);

            UpdateProfile(profile =>
                {
                    profile.InitIfNeeded();
                    _currentSystem = profile.GetCurrentSystemId();
                });
            UpdateDomain(system => system.InitIfNeeded(_currentSystem));

            _queue.Enqueue(new ProfileLoaded(_currentSystem));
            
        }

        void PutStuffInInbox(UI.AddStuffWizardCompleted e)
        {
            _uiBus.Publish(e);
            UpdateDomain(agg => agg.PutStuffInInbox(new RequestId(), e.StuffDescription, new RealTimeProvider()));
        }

        void TrashStuff(UI.TrashStuffClicked e)
        {
            // do something
            _uiBus.Publish(e);
            UpdateDomain(agg => agg.TrashStuff(e.Id, new RealTimeProvider()));
        }

        void DefineProject(UI.DefineNewProjectWizardCompleted r)
        {
            _uiBus.Publish(r);
            var newGuid = Guid.NewGuid();
            UpdateDomain(agg => agg.DefineProject(new RequestId(newGuid), r.Outcome, new RealTimeProvider()));
            _queue.Enqueue(new UI.DisplayProject(new ProjectId(newGuid)));
        }

        void CompleteAction(UI.CompleteActionClicked r)
        {
            UpdateDomain(a => a.CompleteAction(r.Id, new RealTimeProvider()));
        }

        void ChangeOutcome(UI.ChangeActionOutcome r)
        {
            UpdateDomain(a => a.ChangeActionOutcome(r.ActionId,r.Outcome, new RealTimeProvider()));
        }

        void MoveStuffToProject(UI.MoveStuffToProjectClicked e)
        {
            _uiBus.Publish(e);
            UpdateDomain(agg => agg.MoveStuffToProject(e.StuffToMove, e.Project, new RealTimeProvider()));
        }

        void PassThroughEvent(Event e)
        {
            _uiBus.Publish(e);
        }

        
        // lifetime change management

        // atomic consistency boundary of an Aggregate & its contents

        void UpdateDomain(Action<TrustedSystemAggregate> usingThisMethod)
        {
            if (_currentSystem == null)
                throw new InvalidOperationException("System ID should be provided by now");

            var eventStreamId = _currentSystem.ToStreamId();
            var eventStream = _eventStore.LoadEventStream(eventStreamId);

            var aggStateBeforeChanges = TrustedSystemState.BuildStateFromEventHistory(eventStream.Events);

            var aggToChange = new TrustedSystemAggregate(aggStateBeforeChanges);

            
            usingThisMethod(aggToChange);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggToChange.EventsCausingChanges);
        }


        void UpdateProfile(Action<ClientProfileAggregate> updateProfile)
        {
            var eventStreamId = ".profile";
            var eventStream = _eventStore.LoadEventStream(eventStreamId);
            var aggStateBeforeChanges = ClientProfileState.BuildStateFromEventHistory(eventStream.Events);
            var aggToChange = new ClientProfileAggregate(aggStateBeforeChanges);

            updateProfile(aggToChange);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggToChange.EventsCausingChanges);
        }


        public void SetMainQueue(QueuedHandler mainQueue)
        {
            _queue = mainQueue;
        }

        public void Handle(Message message)
        {
            Trace.WriteLine("Handle: " + message.ToString());
            _finiteStateMachine.Handle(message);
        }
    }
}