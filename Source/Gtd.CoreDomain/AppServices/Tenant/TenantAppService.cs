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
            ChangeAggregate(cmd.Id, agg => agg.CaptureThought(cmd.RequestId, cmd.Thought, _time));
        }

        public void When(ArchiveThought cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ArchiveThought(cmd.ThoughtId, _time));
        }

        public void When(DefineAction cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.DefineAction(cmd.RequestId, cmd.ActionName, _time));
        }


        public void When(DefineProject cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.DefineProject(cmd.RequestId, cmd.ProjectOutcome, _time));
        }

        public void When(RemoveAction cmd)
        {
            throw new NotImplementedException();
        }

        public void When(CompleteAction cmd)
        {
            throw new NotImplementedException();
        }

        // lifetime change management & atomic consistency boundary of an Aggregate & its contents
        void ChangeAggregate(TenantId id, Action<TenantAggregate> executeCommandUsingThis)
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

        public void Execute(Command cmd)
        {
            ((dynamic) this).When((dynamic) cmd);
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