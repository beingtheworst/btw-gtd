using System;
using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.TrustedSystem
{
    public sealed class TrustedSystemAggregate
    {
        readonly TrustedSystemState _aggState;

        public List<Event> EventsThatCausedChange = new List<Event>();

        public TrustedSystemAggregate(TrustedSystemState aggregateStateBeforeChanges)
        {
            _aggState = aggregateStateBeforeChanges;
        }

        // Aggregate Behaviors (Methods)

        public void Create(TrustedSystemId id)
        {
            Apply(new TrustedSystemCreated(id));
        }

        public void DefineProject(Guid requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            var defaultProjectType = ProjectType.SingleActions;
            Apply(new ProjectDefined(_aggState.Id, projectId, name, defaultProjectType, time));
        }

        public void CaptureThought(Guid requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            //var time = provider.GetUtcNow();
            //var id = new ActionId(NewGuidIfEmpty(requestId));

            Apply(new ThoughtCaptured(_aggState.Id, NewGuidIfEmpty(requestId), name, provider.GetUtcNow()));
        }

        public void DefineAction(Guid requestId, ProjectId projectId, string outcome, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();

            ProjectInfo info;
            if (!_aggState.Projects.TryGetValue(projectId, out info))
            {
                throw DomainError.Named("unknown-project", "Unknown project {0}", projectId);
            }

            var actionId = new ActionId(NewGuidIfEmpty(requestId));
            Apply(new ActionDefined(_aggState.Id, actionId, projectId, outcome , time));
        }

        public void ArchiveThought(Guid thoughtId, ITimeProvider provider)
        {
            if (!_aggState.Inbox.Contains(thoughtId))
                throw DomainError.Named("no thought", "Thought {0} not found", thoughtId);

            Apply(new ThoughtArchived(_aggState.Id, thoughtId, provider.GetUtcNow()));
        }


        // Helper Methods

        public void CompleteAction(ActionId actionId, ITimeProvider provider)
        {
            ActionInfo info;
            if (!_aggState.Actions.TryGetValue(actionId, out info))
            {
                throw DomainError.Named("unknown action", "Unknown action {0}", actionId);
            }
            if (info.Completed)
                return;// idempotency

            Apply(new ActionCompleted(_aggState.Id, 
                actionId, 
                info.Project, 
                info.Outcome, 
                provider.GetUtcNow()));

        }

        static Guid NewGuidIfEmpty(Guid requestId)
        {
            return requestId == Guid.Empty ? Guid.NewGuid() : requestId;
        }

        /// <summary> Make aggregate realize that the event happened by applying it to the state
        /// and adding to the list of uncommitted events</summary>
        /// <param name="newEventThatHappened"></param>
        void Apply(ITrustedSystemEvent newEventThatHappened)
        {
            // TODO: [kstreet] In the Factory sample these two lines of code were called
            // the other way around (MakeStateRealize was called AFTER the newEventThatHappened was added to List)
            // Does it matter?  Is one approach a little safer/more accurate than the other?

            _aggState.MakeStateRealize(newEventThatHappened);

            EventsThatCausedChange.Add((Event)newEventThatHappened);
        }

        public void ChangeActionOutcome(ActionId actionId, string outcome, ITimeProvider time)
        {
            ActionInfo info;
            if (!_aggState.Actions.TryGetValue(actionId, out info))
            {
                throw DomainError.Named("unknown action", "Unknown action {0}", actionId);
            }
            if (info.Outcome != outcome)
            {
                Apply(new ActionOutcomeChanged(_aggState.Id, actionId, info.Project, outcome, time.GetUtcNow()));
            }
                
        }

        public void ChangeProjectOutcome(ProjectId projectId, string outcome, ITimeProvider time)
        {
            ProjectInfo info;
            if (!_aggState.Projects.TryGetValue(projectId, out info))
            {
                throw DomainError.Named("unknown project", "Unknown project {0}", projectId);
            }
            if (info.Outcome != outcome)
            {
                Apply(new ProjectOutcomeChanged(_aggState.Id, projectId, outcome, time.GetUtcNow()));
            }
        }

        public void ChangeThoughtSubject(Guid thoughtId, string subject, ITimeProvider time)
        {
            ThoughtInfo info;
            if (!_aggState.Thoughts.TryGetValue(thoughtId, out info))
            {
                throw DomainError.Named("unknown thought", "Unknown thought {0}", thoughtId);
            }
            if (info.Subject != subject)
            {
                Apply(new ThoughtSubjectChanged(_aggState.Id, thoughtId, subject, time.GetUtcNow()));
            }
        }

        public void ChangeProjectType(ProjectId projectId, ProjectType type, ITimeProvider time)
        {
            ProjectInfo info;
            if (!_aggState.Projects.TryGetValue(projectId, out info))
            {
                throw DomainError.Named("unknown project", "Unknown project {0}", projectId);
            }
            if (info.Type != type)
            {
                Apply(new ProjectTypeChanged(_aggState.Id, projectId, type, time.GetUtcNow()));
            }
        }
    }
}