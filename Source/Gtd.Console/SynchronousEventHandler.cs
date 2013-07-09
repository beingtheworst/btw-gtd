using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Btw.Portable;
using Gtd.CoreDomain;
using Microsoft.CSharp.RuntimeBinder;

namespace Gtd.Shell
{
    public sealed class SynchronousEventHandler
    {
        readonly IList<object> _eventHandlers = new List<object>();
        public void Handle(Event @event)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                // try to execute each Event that happend against
                // each eventHandler in the list and let it handle the Event if it cares or ignore it if it doesn't
                try
                {
                    ((dynamic)eventHandler).When((dynamic)@event);
                }
                catch (RuntimeBinderException e)
                {
                    // binding failure. Ignore
                }
            }
        }
        public void RegisterHandler(object projection)
        {
            _eventHandlers.Add(projection);
        }
    }

    public sealed class EventStore : IEventStore
    {
        readonly MessageStore _store;
        readonly SynchronousEventHandler _handler;

        public EventStore(MessageStore store, SynchronousEventHandler handler)
        {
            _store = store;
            _handler = handler;
        }

        public void AppendEventsToStream(string name, long streamVersion, ICollection<Event> events)
        {
            if (events.Count == 0) return;
            // functional events don't have an identity

            try
            {
                _store.AppendToStore(name, streamVersion, events.Cast<object>().ToArray());
            }
            catch (AppendOnlyStoreConcurrencyException e)
            {
                // load server events
                var server = LoadEventStream(name);
                // throw a real problem
                throw OptimisticConcurrencyException.Create(server.StreamVersion, e.ExpectedStreamVersion, name, server.Events);
            }
            // sync handling. Normally we would push this to async
            foreach (var @event in events)
            {
                _handler.Handle(@event);
            }
        }
        public EventStream LoadEventStream(string name)
        {
            var list = new List<Event>();
            var streamVersion = 0L;
            foreach (var record in _store.EnumerateMessages(name, 0, int.MaxValue))
            {
                list.AddRange(record.Items.Cast<Event>());
                streamVersion = record.StreamVersion;
            }
            return new EventStream(streamVersion, new ReadOnlyCollection<Event>(list));
        }
    }

}