using System.Collections.Generic;

namespace Gtd.CoreDomain
{
    public interface IAppService
    {
        void Execute(Command command);
    }



    public sealed class EventStream
    {
        public long StreamVersion;
        public List<Event> Events = new List<Event>();
    }

    public interface IEventStore
    {
        EventStream LoadEventStream(string streamId);
        void AppendEventsToStream(string streamId, long expectedStreamVersion, ICollection<Event> events);
    }
}