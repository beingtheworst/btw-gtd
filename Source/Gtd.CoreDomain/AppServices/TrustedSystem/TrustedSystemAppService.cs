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
        void ChangeAgg(TrustedSystemId withAggIdOf, Action<TrustedSystemAggregate> usingThisMethod)
        {
            var eventStreamId = withAggIdOf.Id.ToString();
            var eventStream = _eventStore.LoadEventStream(eventStreamId);

            var aggStateBeforeChanges = TrustedSystemState.BuildStateFromEventHistory(eventStream.Events);

            var aggToChange = new TrustedSystemAggregate(aggStateBeforeChanges);

            // HACK
            if (eventStream.Events.Count == 0)
            {
                aggToChange.Create(withAggIdOf);
            }

            usingThisMethod(aggToChange);

            _eventStore.AppendEventsToStream(eventStreamId, eventStream.StreamVersion, aggToChange.EventsCausingChanges);
        }



        // Ability to Execute Command Messages (IAppService)
        public void Execute(Command cmd)          
        {
            ((dynamic)this).When((dynamic)cmd);
        }


        // When methods reacting to Executed Command to call corresponding Aggregate method

        public void When(PutStuffInInbox cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.PutStuffInInbox(cmd.RequestId, cmd.Stuff, _time));
        }

        public void When(TrashStuff cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.TrashStuff(cmd.StuffId, _time));
        }

        public void When(CaptureThought cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.CaptureThought(cmd.RequestId, cmd.Thought, _time));
        }

        public void When(ArchiveThought cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ArchiveThought(cmd.ThoughtId, _time));
        }

        public void When(DefineAction cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.DefineAction(cmd.RequestId, cmd.ProjectId, cmd.Outcome, _time));
        }

        public void When(DefineProject cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.DefineProject(cmd.RequestId, cmd.ProjectOutcome, _time));
        }

        public void When(DefineSingleActionProject cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.DefineSingleActionProject(cmd.RequestId, cmd.ThoughtId, _time));
        }

        public void When(ChangeProjectType cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ChangeProjectType(cmd.ProjectId, cmd.Type, _time));
        }

        public void When(ArchiveAction cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ArchiveAction(cmd.ActionId, _time));
        }

        public void When(CompleteAction cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.CompleteAction(cmd.ActionId, _time));
        }

        public void When(ChangeActionOutcome cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ChangeActionOutcome(cmd.ActionId, cmd.Outcome, _time));
        }

        public void When(ChangeProjectOutcome cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ChangeProjectOutcome(cmd.ProjectId, cmd.Outcome, _time));
        }

        public void When(ChangeThoughtSubject cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ChangeThoughtSubject(cmd.ThoughtId, cmd.Subject, _time));
        }

        public void When(ProvideStartDateForAction cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ProvideStartDateForAction(cmd.ActionId, cmd.NewStartDate));
        }

        public void When(ProvideDueDateForAction cmd)
        {
            ChangeAgg(cmd.Id, agg => agg.ProvideDueDateForAction(cmd.ActionId, cmd.NewDueDate));
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