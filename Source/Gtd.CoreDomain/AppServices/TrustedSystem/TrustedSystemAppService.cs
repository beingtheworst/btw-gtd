using System;
using Gtd.CoreDomain.AppServices.TrustedSystem;

namespace Gtd.CoreDomain
{
    public sealed class TrustedSystemAppService : ITrustedSystemApplicationService, IAppService
    {
        readonly IEventStore _eventStore;
        readonly ITimeProvider _time;

        public TrustedSystemAppService(IEventStore eventStore, ITimeProvider time)
        {
            _eventStore = eventStore;
            _time = time;
        }

        // lifetime change management
        // atomic consistency boundary of an Aggregate & its contents
        void ChangeAggregate(TrustedSystemId aggregateIdOf, Action<TrustedSystemAggregate> usingThisMethod)
        {
            var eventStreamId = aggregateIdOf.Id.ToString();
            var eventStream = _eventStore.LoadEventStream(eventStreamId);

            var aggStateBeforeChanges = TrustedSystemState.BuildStateFromEventHistory(eventStream.Events);

            var aggregateToChange = new TrustedSystemAggregate(aggStateBeforeChanges);

            // HACK
            if (eventStream.Events.Count == 0)
            {
                aggregateToChange.Create(aggregateIdOf);
            }

            usingThisMethod(aggregateToChange);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggregateToChange.EventsThatCausedChange);
        }



        // Ability to Execute Command Messages (IAppService)
        public void Execute(Command cmd)          
        {
            ((dynamic)this).When((dynamic)cmd);
        }


        // When methods reacting to Executed Command to call corresponding Aggregate method

        public void When(CaptureInnoxStuff cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.CaptureThought(cmd.RequestId, cmd.Subject, _time));
        }

        public void When(ArchiveInboxStuff cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ArchiveThought(cmd.InboxStuffId, _time));
        }

        public void When(DefineAction cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.DefineAction(cmd.RequestId, cmd.ProjectId, cmd.Outcome, _time));
        }

        public void When(DefineProject cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.DefineProject(cmd.RequestId, cmd.ProjectOutcome, _time));
        }

        public void When(DefineSingleActionProject cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.DefineSingleActionProject(cmd.RequestId, cmd.InboxStuffId, _time));
        }

        public void When(ChangeProjectType cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ChangeProjectType(cmd.ProjectId, cmd.Type, _time));
        }

        public void When(ArchiveAction cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ArchiveAction(cmd.ActionId, _time));
        }

        public void When(CompleteAction cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.CompleteAction(cmd.ActionId, _time));
        }

        public void When(ChangeActionOutcome cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ChangeActionOutcome(cmd.ActionId, cmd.Outcome, _time));
        }

        public void When(ChangeProjectOutcome cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ChangeProjectOutcome(cmd.ProjectId, cmd.Outcome, _time));
        }

        public void When(ChangeSubjectOfInboxStuff cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ChangeThoughtSubject(cmd.InboxStuffId, cmd.Subject, _time));
        }

        public void When(DeferActionUntil cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.DeferActionUntil(cmd.ActionId, cmd.DeferUntil));
        }

        public void When(ProvideDueDateForAction cmd)
        {
            ChangeAggregate(cmd.Id, agg => agg.ProvideDueDateForAction(cmd.ActionId, cmd.NewDueDate));
        }

    }


    // TODO: [kstreet] when we add Aggregate #2 
    // will we want to move this general "Time" stuff?
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