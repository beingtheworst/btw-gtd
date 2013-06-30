using System;
using System.Diagnostics;
using Gtd.CoreDomain;
using Gtd.CoreDomain.AppServices.TrustedSystem;

namespace Gtd.Client
{
    public sealed class AppController : IHandle<Message>
    {
        IPublisher _bus;
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
                    .When<InboxShown>().Do(shown => _state = AppState.InboxView)
                    .When<RequestCapture>().Do(CaptureThought)
                    .When<RequestRemove>().Do(RemoveCapturedThought)
                .InState(AppState.Loading)
                    .When<ShowInbox>().Do(_bus.Publish)
                    .When<FormLoad>().Do(Deal)
                    .When<AppInit>().Do(_bus.Publish)
                    .When<Event>().Do(PassThroughEvent)
                .InState(AppState.InboxView)
                .WhenOther().Do(_bus.Publish)
                    
                
                
                .Build(() => (int) _state);
        }

        void RemoveCapturedThought(RequestRemove r)
        {
            // do something
            _bus.Publish(r);
            ChangeAggregate(a => a.ArchiveThought(r.Id,new RealTimeProvider()));
        }

        void CaptureThought(RequestCapture c)
        {
            _bus.Publish(c);

            ChangeAggregate(aggregate => aggregate.CaptureThought(new RequestId(), "test", new RealTimeProvider() ));
        }

        void PassThroughEvent(Event e)
        {
            _bus.Publish(e);
        }

        void Deal(FormLoad obj)
        {
            _bus.Publish(obj);

            // freeze UI somehow

            foreach (var e in _store.LoadEventStream("app").Events)
            {
                _mainQueue.Enqueue(e);
            }

            _mainQueue.Enqueue(new ShowInbox());

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