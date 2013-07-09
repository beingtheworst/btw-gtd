using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Gtd.CoreDomain;

namespace Gtd.Shell
{
    public sealed class InMemoryStore : IEventStore
    {
        readonly ConcurrentDictionary<string, IList<Event>> _eventStore = new ConcurrentDictionary<string, IList<Event>>();
        readonly SynchronousEventHandler _eventHandler;

        //static ILogger Log = LogManager.GetLoggerFor<InMemoryStore>();
        public InMemoryStore(SynchronousEventHandler eventHandler)
        {
            _eventHandler = eventHandler;
        }

        public EventStream LoadEventStream(string id)
        {
            var eventStream = _eventStore.GetOrAdd(id, new Event[0]);

            return new EventStream(eventStream.Count, new ReadOnlyCollection<Event>(eventStream));
        }

        public void AppendEventsToStream(string id, long expectedVersion, ICollection<Event> events)
        {
            _eventStore.AddOrUpdate(id, events.ToList(), (s, list) => list.Concat(events).ToList());

            // to make the example simple, right after we "persist" the Events to the store above
            // we immediately call the projections that handle Events.

            foreach (var @event in events)
            {
                //Log.Info("{0}", @event);

                // call the eventHandler so that all subscribed projections 
                // can "realize and react to" the Events that happend
                _eventHandler.Handle(@event);
            }
        }
    }
}