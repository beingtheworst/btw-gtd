using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Gtd.CoreDomain
{
    public interface IAppService
    {
        void Execute(Command command);
    }



    public sealed class EventStream
    {
        public EventStream(long streamVersion, ReadOnlyCollection<Event> events)
        {
            StreamVersion = streamVersion;
            Events = (events);
        }

        public readonly long StreamVersion;
        public readonly IList<Event> Events;
    }

    public interface IEventStore
    {
        EventStream LoadEventStream(string streamId);
        void AppendEventsToStream(string streamId, long expectedStreamVersion, ICollection<Event> events);
    }

    /// <summary>
    /// Is thrown by event store if there were changes since our last version
    /// </summary>
    [Serializable]
    public class OptimisticConcurrencyException : Exception
    {
        public long ActualVersion { get; private set; }
        public long ExpectedVersion { get; private set; }
        public string Name { get; private set; }
        public IList<Event> ActualEvents { get; private set; }

        OptimisticConcurrencyException(string message, long actualVersion, long expectedVersion, string name,
            IList<Event> serverEvents)
            : base(message)
        {
            ActualVersion = actualVersion;
            ExpectedVersion = expectedVersion;
            Name = name;
            ActualEvents = serverEvents;
        }

        public static OptimisticConcurrencyException Create(long actual, long expected, string name,
            IList<Event> serverEvents)
        {
            var message = string.Format("Expected v{0} but found v{1} in stream '{2}'", expected, actual, name);
            return new OptimisticConcurrencyException(message, actual, expected, name, serverEvents);
        }

        protected OptimisticConcurrencyException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context) { }
    }

}