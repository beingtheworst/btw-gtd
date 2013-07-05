using System;
using System.Diagnostics;
using Gtd.Client.Views.Actions;
using Gtd.CoreDomain;
using Gtd.CoreDomain.AppServices.TrustedSystem;

namespace Gtd.Client
{
    public sealed class AppController : IHandle<Message>
    {
        readonly IPublisher _bus;
        readonly IEventStore _store;
        QueuedHandler _mainQueue;

        readonly FiniteStateMachine<AppState> _fsm;

        public AppController(IPublisher bus, IEventStore store)
        {
            _bus = bus;
            _store = store;


            _fsm = CreateFsm();

            
        }

        AppState _state = AppState.Loading;

        FiniteStateMachine<AppState> CreateFsm()
        {
            return new FsmBuilder<AppState>()
                .InAllStates()
                    .When<Ui.InboxShown>().Do(shown => _state = AppState.InboxView)
                    .When<Ui.CaptureThought>().Do(Handle)
                    .When<Ui.ArchiveThought>().Do(Handle)
                    .When<Ui.DefineNewProject>().Do(Handle)
                    .When<Ui.MoveThoughtsToProject>().Do(Handle)
                    .When<Ui.CompleteAction>().Do(Handle)
                .InState(AppState.Loading)
                    .When<Ui.DisplayInbox>().Do(_bus.Publish)
                    .When<FormLoaded>().Do(_bus.Publish)
                    .When<FormLoading>().Do(Deal)
                    .When<AppInit>().Do(_bus.Publish)
                    .When<Event>().Do(PassThroughEvent)
                .InState(AppState.InboxView)
                .WhenOther().Do(_bus.Publish)
                    
                
                
                .Build(() => (int) _state);
        }

        void Handle(Ui.DefineNewProject r)
        {
            _bus.Publish(r);
            ChangeAggregate(a => a.DefineProject(new RequestId(), r.Outcome, new RealTimeProvider() ));
        }

        void Handle(Ui.CompleteAction r)
        {
            ChangeAggregate(a => a.CompleteAction(r.Id, new RealTimeProvider()));
        }

        void Handle(Ui.MoveThoughtsToProject r)
        {
            _bus.Publish(r);
            ChangeAggregate(a => a.MoveThoughtsToProject(r.Thoughts, r.Project, new RealTimeProvider()));
        }

        void Handle(Ui.ArchiveThought r)
        {
            // do something
            _bus.Publish(r);
            ChangeAggregate(a => a.ArchiveThought(r.Id,new RealTimeProvider()));
        }

        void Handle(Ui.CaptureThought c)
        {
            _bus.Publish(c);

            ChangeAggregate(aggregate => aggregate.CaptureThought(new RequestId(), c.Thought, new RealTimeProvider() ));
        }

        void PassThroughEvent(Event e)
        {
            _bus.Publish(e);
        }

        void Deal(FormLoading obj)
        {
            _bus.Publish(obj);


            _mainQueue.Enqueue(new FormLoaded());
            _mainQueue.Enqueue(new Ui.DisplayInbox());

        }

        // lifetime change management

        // atomic consistency boundary of an Aggregate & its contents

        void ChangeAggregate(Action<TrustedSystemAggregate> usingThisMethod)
        {
            var eventStreamId = "app";
            var eventStream = _store.LoadEventStream(eventStreamId);

            var aggStateBeforeChanges = TrustedSystemState.BuildStateFromEventHistory(eventStream.Events);

            var aggregateToChange = new TrustedSystemAggregate(aggStateBeforeChanges);

            // HACK
            if (eventStream.Events.Count == 0)
            {
                aggregateToChange.Create(new TrustedSystemId(0));
            }

            usingThisMethod(aggregateToChange);

            _store.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregateToChange.EventsThatCausedChange);
        }


        public void SetMainQueue(QueuedHandler mainQueue)
        {
            _mainQueue = mainQueue;
        }

        public void Handle(Message message)
        {
            Trace.WriteLine("Handle: " + message.ToString());
            _fsm.Handle(message);
        }
    }
}