using System;
using Gtd.CoreDomain.AppServices.Tenant;

namespace Gtd.CoreDomain
{
    public sealed class TenantAppService : ITenantApplicationService, IAppService
    {
        readonly IEventStore _eventStore;
        readonly ITimeProvider _time;


        public TenantAppService(IEventStore eventStore, ITimeProvider time)
        {
            _eventStore = eventStore;
            _time = time;
        }


        public void When(CaptureInboxEntry c)
        {
            Update(c.Id, a => a.CaptureInboxEntry(c.RequestId, c.Name, _time));
        }

        public void When(DefineAction c)
        {
            Update(c.Id, a => a.DefineAction(c.RequestId, c.ActionName, _time));
        }


        public void When(DefineProject c)
        {
            Update(c.Id, a => a.DefineProject(c.RequestId, c.ProjectOutcome,_time));
        }

        public void When(RemoveAction c)
        {
            throw new NotImplementedException();
        }

        public void When(CompleteAction c)
        {
            throw new NotImplementedException();
        }

        void Update(TenantId id, Action<TenantAggregate> executeCommandUsingThis)
        {
            var streamId = id.Id.ToString();
            var eventStream = _eventStore.LoadEventStream(streamId);
            var state = TenantState.BuildStateFromHistory(eventStream.Events);
            var aggregate = new TenantAggregate(state);
            executeCommandUsingThis(aggregate);
            _eventStore.AppendEventsToStream(streamId, eventStream.StreamVersion, aggregate.EventsThatHappened);
        }

        public void Execute(Command command)
        {
            ((dynamic) this).When((dynamic) command);
        }
    }

    public interface ITimeProvider
    {
        DateTime GetUtcNow();
    }

    public sealed class RealTimeProvider : ITimeProvider
    {
        public DateTime GetUtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}