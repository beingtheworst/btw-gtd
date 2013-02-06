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


        public void When(CaptureThought cmd)
        {
            Update(cmd.Id, a => a.CaptureThought(cmd.RequestId, cmd.Thought, _time));
        }

        public void When(ArchiveThought cmd)
        {
            Update(cmd.Id, a => a.ArchiveThought(cmd.ThoughtId, _time));
        }

        public void When(DefineAction cmd)
        {
            Update(cmd.Id, a => a.DefineAction(cmd.RequestId, cmd.ActionName, _time));
        }


        public void When(DefineProject cmd)
        {
            Update(cmd.Id, a => a.DefineProject(cmd.RequestId, cmd.ProjectOutcome,_time));
        }

        public void When(RemoveAction cmd)
        {
            throw new NotImplementedException();
        }

        public void When(CompleteAction cmd)
        {
            throw new NotImplementedException();
        }

        void Update(TenantId id, Action<TenantAggregate> executeCommandUsingThis)
        {
            var streamId = id.Id.ToString();
            var eventStream = _eventStore.LoadEventStream(streamId);




            var state = TenantState.BuildStateFromHistory(eventStream.Events);

            
            var aggregate = new TenantAggregate(state);

            // HACK
            if (eventStream.Events.Count == 0)
            {
                aggregate.Create(id);
            }

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