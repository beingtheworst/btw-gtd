﻿using System;
using System.Diagnostics;
using Gtd.CoreDomain;
using Gtd.CoreDomain.AppServices.ClientProfile;
using Gtd.CoreDomain.AppServices.TrustedSystem;

namespace Gtd.Client
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

        readonly FiniteStateMachine<AppState> _fsm;

        public AppController(IPublisher uiBus, IEventStore eventStore)
        {
            _uiBus = uiBus;
            _eventStore = eventStore;

            _fsm = CreateStateMachine();
        }

        // default Fsm's intial state to "Loading"
        AppState _appState = AppState.Loading;

        #region Whats a Finite State Machine (Fsm)...
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

                    .When<Ui.CaptureThoughtWizardCompleted>().Do(CaptureThought)
                    .When<Ui.ArchiveThoughtClicked>().Do(ArchiveThought)
                    .When<Ui.DefineNewProjectWizardCompleted>().Do(DefineProject)
                    .When<Ui.MoveThoughtsToProjectClicked>().Do(MoveThoughtsToProject)
                    .When<Ui.CompleteActionClicked>().Do(CompleteAction)
                    

                .InState(AppState.Loading)
                    .When<Ui.DisplayInbox>().Do(_uiBus.Publish)
                    .When<FormLoaded>().Do(_uiBus.Publish)
                    .When<FormLoading>().Do(Deal)
                    .When<AppInit>().Do(InitApplication)
                    .When<Event>().Do(PassThroughEvent)
                    
                .WhenOther().Do(_uiBus.Publish)
                .Build(() => (int) _appState);
        }

        TrustedSystemId _currentSystem;

        void InitApplication(AppInit obj)
        {
            _uiBus.Publish(obj);

            UpdateProfile(profile =>
                {
                    profile.InitIfNeeded();
                    _currentSystem = profile.GetCurrentSustemId();
                });
            UpdateDomain(system => system.InitIfNeeded(_currentSystem));

            _queue.Enqueue(new ProfileLoaded(_currentSystem));
            
        }

        void DefineProject(Ui.DefineNewProjectWizardCompleted r)
        {
            _uiBus.Publish(r);
            var newGuid = Guid.NewGuid();
            UpdateDomain(a => a.DefineProject(new RequestId(newGuid), r.Outcome, new RealTimeProvider()));
            _queue.Enqueue(new Ui.DisplayProject(new ProjectId(newGuid)));
        }

        void CompleteAction(Ui.CompleteActionClicked r)
        {
            UpdateDomain(a => a.CompleteAction(r.Id, new RealTimeProvider()));
        }

        void MoveThoughtsToProject(Ui.MoveThoughtsToProjectClicked r)
        {
            _uiBus.Publish(r);
            UpdateDomain(a => a.MoveThoughtsToProject(r.Thoughts, r.Project, new RealTimeProvider()));
        }

        void ArchiveThought(Ui.ArchiveThoughtClicked r)
        {
            // do something
            _uiBus.Publish(r);
            UpdateDomain(a => a.ArchiveThought(r.Id,new RealTimeProvider()));
        }

        void CaptureThought(Ui.CaptureThoughtWizardCompleted c)
        {
            _uiBus.Publish(c);

            UpdateDomain(aggregate => aggregate.CaptureThought(new RequestId(), c.Thought, new RealTimeProvider() ));
        }

        void PassThroughEvent(Event e)
        {
            _uiBus.Publish(e);
        }

        void Deal(FormLoading obj)
        {
            _uiBus.Publish(obj);

            _queue.Enqueue(new FormLoaded());
            _queue.Enqueue(new Ui.DisplayInbox());
        }

        // lifetime change management

        // atomic consistency boundary of an Aggregate & its contents

        void UpdateDomain(Action<TrustedSystemAggregate> usingThisMethod)
        {
            if (_currentSystem == null)
                throw new InvalidOperationException("System ID should be provided by now");

            var eventStreamId = _currentSystem.ToStreamId();
            var eventStream = _eventStore.LoadEventStream(eventStreamId);

            var state = TrustedSystemState.BuildStateFromEventHistory(eventStream.Events);

            var aggregateToChange = new TrustedSystemAggregate(state);

            
            usingThisMethod(aggregateToChange);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregateToChange.EventsThatCausedChange);
        }


        void UpdateProfile(Action<ClientProfileAggregate> updateProfile)
        {
            var eventStreamId = ".profile";
            var eventStream = _eventStore.LoadEventStream(eventStreamId);
            var state = ClientProfileState.BuildStateFromEventHistory(eventStream.Events);
            var aggregate = new ClientProfileAggregate(state);

            updateProfile(aggregate);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregate.Changes);
        }


        public void SetMainQueue(QueuedHandler mainQueue)
        {
            _queue = mainQueue;
        }

        public void Handle(Message message)
        {
            Trace.WriteLine("Handle: " + message.ToString());
            _fsm.Handle(message);
        }
    }
}