using System;
using Gtd.CoreDomain.AppServices.Tenant;

namespace Gtd.CoreDomain
{
    public sealed class TenantAppService : ITenantApplicationService, IAppService
    {
        readonly IEventStore _eventStore;
        readonly ITimeProvider _time;


        public TenantAppService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }


        public void When(CaptureAction c)
        {

            Update(c.Id, a => a.CaptureAction(c.RequestId, c.Name, _time));
        }

        public void When(CreateProject c)
        {
            Update(c.Id, a => a.CreateProject(c.RequestId, c.Name,_time));
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

        void IAppService.Execute(Command command)
        {
            ((dynamic) this).When((dynamic) command);
        }
    }

    public interface ITimeProvider
    {
        DateTime GetUtcNow();
    }
}