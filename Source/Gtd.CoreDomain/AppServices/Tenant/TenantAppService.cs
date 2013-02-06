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
        void ChangeAggregate(TenantId aggregateIdOf, Action<TenantAggregate> usingThisMethod)
        {
            var eventStreamId = aggregateIdOf.Id.ToString();
            var eventStream = _eventStore.LoadEventStream(eventStreamId);

            var aggStateBeforeChanges = TenantState.BuildStateFromEventHistory(eventStream.Events);


            var aggregateToChange = new TenantAggregate(aggStateBeforeChanges);

            // HACK
            if (eventStream.Events.Count == 0)
            {
                aggregateToChange.Create(aggregateIdOf);
            }

            usingThisMethod(aggregateToChange);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregateToChange.EventsThatCausedChange);
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