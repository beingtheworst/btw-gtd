using System;
using System.Diagnostics;
using Gtd.CoreDomain;
using Gtd.CoreDomain.AppServices.ClientProfile;
using Gtd.CoreDomain.AppServices.TrustedSystem;

namespace Gtd.Client
{
    public sealed class AppController : IHandle<Message>
    {
        readonly IPublisher _bus;
        readonly IEventStore _store;
        QueuedHandler _queue;

        readonly FiniteStateMachine<AppState> _fsm;

        public AppController(IPublisher bus, IEventStore store)
        {
            _bus = bus;
            _store = store;

            _fsm = CreateStateMachine();
        }

        AppState _state = AppState.Loading;

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
                    .When<Ui.DisplayInbox>().Do(_bus.Publish)
                    .When<FormLoaded>().Do(_bus.Publish)
                    .When<FormLoading>().Do(Deal)
                    .When<AppInit>().Do(InitApplication)
                    .When<Event>().Do(PassThroughEvent)
                    
                .WhenOther().Do(_bus.Publish)
                .Build(() => (int) _state);
        }

        TrustedSystemId _currentSystem;

        void InitApplication(AppInit obj)
        {
            _bus.Publish(obj);

            UpdateProfile(profile =>
                {
                    profile.InitIfNeeded();
                    _currentSystem = profile.GetCurrentSustemId();
                });
            UpdateDomain(system => system.InitIfNeeded(_currentSystem));
            
        }

        void DefineProject(Ui.DefineNewProjectWizardCompleted r)
        {
            _bus.Publish(r);
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
            _bus.Publish(r);
            UpdateDomain(a => a.MoveThoughtsToProject(r.Thoughts, r.Project, new RealTimeProvider()));
        }

        void ArchiveThought(Ui.ArchiveThoughtClicked r)
        {
            // do something
            _bus.Publish(r);
            UpdateDomain(a => a.ArchiveThought(r.Id,new RealTimeProvider()));
        }

        void CaptureThought(Ui.CaptureThoughtWizardCompleted c)
        {
            _bus.Publish(c);

            UpdateDomain(aggregate => aggregate.CaptureThought(new RequestId(), c.Thought, new RealTimeProvider() ));
        }

        void PassThroughEvent(Event e)
        {
            _bus.Publish(e);
        }

        void Deal(FormLoading obj)
        {
            _bus.Publish(obj);

            _queue.Enqueue(new FormLoaded());
            _queue.Enqueue(new Ui.DisplayInbox());
        }

        // lifetime change management

        // atomic consistency boundary of an Aggregate & its contents

        void UpdateDomain(Action<TrustedSystemAggregate> usingThisMethod)
        {
            if (_currentSystem == null)
                throw new InvalidOperationException("System ID should be provided by now");

            var eventStreamId = "system-" + _currentSystem.Id;
            var eventStream = _store.LoadEventStream(eventStreamId);

            var state = TrustedSystemState.BuildStateFromEventHistory(eventStream.Events);

            var aggregateToChange = new TrustedSystemAggregate(state);

            
            usingThisMethod(aggregateToChange);

            _store.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregateToChange.EventsThatCausedChange);
        }

        

        void UpdateProfile(Action<ClientProfileAggregate> updateProfile)
        {
            var eventStreamId = ".profile";
            var eventStream = _store.LoadEventStream(eventStreamId);
            var state = ClientProfileState.BuildStateFromEventHistory(eventStream.Events);
            var aggregate = new ClientProfileAggregate(state);

            updateProfile(aggregate);

            _store.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregate.Changes);
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