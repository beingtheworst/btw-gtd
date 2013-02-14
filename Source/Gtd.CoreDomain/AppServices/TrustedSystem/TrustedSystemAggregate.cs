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

        public void DefineSingleActionProject(Guid requestId, Guid thoughtId, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            // generate actionId
            var actionId = new ActionId(Guid.NewGuid());

            // make sure thought exists
            ThoughtInfo info;
            if (!_aggState.Thoughts.TryGetValue(thoughtId, out info))
            {
                throw DomainError.Named("unknown thought", "Unknown thought {0}", thoughtId);
            }
            // TODO: May be able to use this to change the thought subject and then let that cascade down
            // as both the Project AND Action Outcome in case you wanted to use a different name from original thought
            // With current design it may be better to kick off a "RENAME" command or Apply Renamed event for that purpose though.
            //if (info.Subject != subject)
            //{
            //    Apply(new ThoughtSubjectChanged(_aggState.Id, thoughtId, subject, time.GetUtcNow()));
            //}

            // TODO: Not sure if it best to just reuse existing Events and projections (probably)
            // or if I shoudl create a new composite event for this new command msg.  Thinking the former, not latter is way to go.
            Apply(new ProjectDefined(_aggState.Id, projectId, info.Subject, ProjectType.SingleActions, time));
            Apply(new ActionDefined(_aggState.Id, actionId, projectId, info.Subject, time));
            //Apply(new SingleActionProjectDefined(_aggState.Id, projectId, info.Subject, ProjectType.SingleActions, actionId, info.Subject, time));

            // Maybe Archive the thought from the inbox too?
            Apply(new ThoughtArchived(_aggState.Id, thoughtId, time));
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

        ActionInfo GetActionOrThrow(ActionId id)
        {
            ActionInfo info;
            if (!_aggState.Actions.TryGetValue(id, out info))
            {
                throw DomainError.Named("unknown action", "Unknown action {0}", id);
            }
            return info;

        }

        public void ArchiveAction(ActionId actionId, ITimeProvider time)
        {
            var action = GetActionOrThrow(actionId);
            if (!action.Archived)
            {
                Apply(new ActionArchived(_aggState.Id, actionId, action.Project, time.GetUtcNow()));
            }
        }




        //
        // Aggregate Helper Methods Below
        //

        static Guid NewGuidIfEmpty(Guid requestId)
        {
            return requestId == Guid.Empty ? Guid.NewGuid() : requestId;
        }

        /// <summary> Make aggregate realize that the event happened by applying it to the state
        /// and adding to the list of uncommitted events</summary>
        /// <param name="newEventThatHappened"></param>
        void Apply(ITrustedSystemEvent newEventThatHappened)
        {
            _aggState.MakeStateRealize(newEventThatHappened);

            EventsThatCausedChange.Add((Event)newEventThatHappened);
        }

        public void ProvideStartDateForAction(ActionId actionId, DateTime newStartDate)
        {
            var action = GetActionOrThrow(actionId);
            if (newStartDate == action.StartDate)
                return;
            if (newStartDate == DateTime.MinValue)
            {
                Apply(new StartDateRemovedFromAction(_aggState.Id, actionId, action.StartDate));
                return;
            }
            
            if (action.DueDate != DateTime.MinValue && newStartDate > action.DueDate)
            {
                throw DomainError.Named("", "New start date is later than due date");
            }


            if (action.StartDate == DateTime.MinValue)
            {
                Apply(new StartDateAssignedToAction(_aggState.Id, actionId, newStartDate));
                return;
            }
            Apply(new ActionStartDateMoved(_aggState.Id, actionId, action.StartDate, newStartDate));
        }

        public void ProvideDueDateForAction(ActionId actionId, DateTime newDueDate)
        {
            var action = GetActionOrThrow(actionId);
            if (newDueDate == action.DueDate)
                return;
            if (newDueDate == DateTime.MinValue)
            {
                Apply(new DueDateRemovedFromAction(_aggState.Id, actionId, action.DueDate));
                return;
            }

            if (action.StartDate != DateTime.MinValue && newDueDate < action.StartDate)
            {
                throw DomainError.Named("", "New due date is earlier than start date");
            }


            if (action.DueDate == DateTime.MinValue)
            {
                Apply(new DueDateAssignedToAction(_aggState.Id, actionId, newDueDate));
                return;
            }
            Apply(new ActionDueDateMoved(_aggState.Id, actionId, action.DueDate, newDueDate));
        }
    }
}