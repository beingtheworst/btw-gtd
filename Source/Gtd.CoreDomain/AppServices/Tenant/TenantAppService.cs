using System;
using System.Collections.Generic;
using Gtd.CoreDomain.AppServices.Tenant;

namespace Gtd.CoreDomain
{
    public sealed class TenantAppService : ITenantApplicationService, IAppService
    {
        readonly IEventStore _eventStore;


        public TenantAppService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }


        public void When(CaptureAction c)
        {
            
        }

        public void When(CreateProject c)
        {
            Update(c.Id, a => a.CreateProject(c.Name));
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

    public sealed class TenantAggregate
    {
        readonly TenantState _state;

        public List<Event> EventsThatHappened = new List<Event>();

        public TenantAggregate(TenantState state)
        {
            _state = state;
        }

        public void CreateProject(string name)
        {
            throw new NotImplementedException();
        }
    }
}