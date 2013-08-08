using System;
using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.TrustedSystem
{
    /// <summary> This is the behavioral half of our A+ES implementation.
    /// 
    /// We split the Aggregates and Event Sourcing (A+ES) implementation
    /// into two distinct classes:
    /// - one for state (AggregateNameState),
    /// - and one for behavior (AggregateName),
    /// with the state object being held by the behavioral object (this Aggregate class).
    /// 
    /// The two objects collaborate exclusively through the Aggregate's Apply() method.
    /// This ensures that an Aggregate's state is changed only by means of Events.
    /// When Events cause state changes to an Aggregate, 
    /// the instant in-memory realization and reflection of this is in AggregateNameState,
    /// and the durable reflection of these changes are persisted back to 
    /// the Event Stream by the ApplicationService.  The AppService is what loads and calls 
    /// the Aggregate and its behaviors, and acts as the
    /// atomic consistency boundary of an Aggregate & its contents.
    /// </summary>
    public sealed class TrustedSystemAggregate
    {
        readonly TrustedSystemState _aggState;

        public List<Event> EventsCausingChanges = new List<Event>();

        public TrustedSystemAggregate(TrustedSystemState aggStateBeforeChanges)
        {
            _aggState = aggStateBeforeChanges;
        }

        /// <summary> Make Aggregate realize the event happened 
        /// by applying it to the state and adding to 
        /// the list of uncommitted events</summary>
        /// <param name="eventThatHappened"></param>
        void Apply(ITrustedSystemEvent eventThatHappened)
        {
            // update Agg's in-memory state so if a behavior (method) has 
            // multiple steps, each subsequent step has up-to-date state to operate on
            _aggState.MakeStateRealize(eventThatHappened);

            // update Agg's public collection of change causing Events so the 
            // AppService can use it to persist AggState as appended Events to this Agg's Event Stream
            EventsCausingChanges.Add((Event)eventThatHappened);
        }


        static Guid NewGuidIfEmpty(RequestId requestId)
        {
            return requestId.IsEmpty ? new RequestId(Guid.NewGuid()).Id : requestId.Id;
        }


        // Aggregate-Specific Behaviors (Methods) Below

        public void InitIfNeeded(TrustedSystemId id)
        {
            if (_aggState.Id == null)
            {
                Create(id);
            }
        }

        public void Create(TrustedSystemId id)
        {
            Apply(new TrustedSystemCreated(id));
        }

        public void PutStuffInInbox(RequestId requestId, string stuffDescription, ITimeProvider provider)
        {
            var stuffId = new StuffId(NewGuidIfEmpty(requestId));
            Apply(new StuffPutInInbox(_aggState.Id, stuffId, stuffDescription, provider.GetUtcNow()));
        }

        public void ChangeDescriptionOfStuff(StuffId stuffId, string newDescriptionOfStuff, ITimeProvider time)
        {
            StuffInfo stuffInfo;
            if (!_aggState.StuffInInbox.TryGetValue(stuffId, out stuffInfo))
            {
                throw DomainError.Named("unknown stuff", "Unknown stuff {0}", stuffId);
            }
            if (stuffInfo.Description != newDescriptionOfStuff)
            {
                Apply(new StuffDescriptionChanged(_aggState.Id, stuffId, newDescriptionOfStuff, time.GetUtcNow()));
            }
        }

        // Trash vs. Archives
        // Archiving is an internal housekeeping process.
        // It is an event that happens within the system as a result of another user command.
        // A user doesn't "Archive" something, the system does.
        // A user Trashes stuff, puts stuff in their ReferenceSystem, Puts stuff In SomedayMaybe, MovesToProject, etc.
        // If a user Moves Stuff from the Inbox to a Project, then StuffArchived from Inbox, not StuffTrashed.
        // If a user wants to "View Archives" then things that were Archived by the system appear in that View.
        // However, if a user expressed their intent to TRASH an item, it is NOT in the "Archived Views".
        // The trashed stuff is still in the Event Stream, but from the user's perspective, it is GONE/TRASHED/Not visible!

        // This Event happens when Stuff is intentionally "trashed" by the user because they want it "out" of the system
        public void TrashStuff(StuffId stuffId, ITimeProvider provider)
        {
            if (!_aggState.Inbox.Contains(stuffId))
                throw DomainError.Named("no stuff", "Stuff with Id {0} not found", stuffId.Id);

            Apply(new StuffTrashed(_aggState.Id, stuffId, provider.GetUtcNow()));
        }

        
        // This Event happens when Stuff is moved from the Inbox to another part of the system
        // TODO: Not sure if I need a method for a Command to call because StuffArchived Event is system generated.
        //public void ArchiveStuff(StuffId stuffId, ITimeProvider provider)
        //{
        //    if (!_aggState.Inbox.Contains(stuffId))
        //        throw DomainError.Named("no stuff", "Stuff with Id {0} not found", stuffId);

        //    Apply(new StuffArchived(_aggState.Id, stuffId, provider.GetUtcNow()));
        //}


        public void DefineProject(RequestId requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            const ProjectType defaultProjectType = ProjectType.List;
            Apply(new ProjectDefined(_aggState.Id, projectId, name, defaultProjectType, time));
        }

        public void DefineSingleActionProject(RequestId requestId, StuffId stuffId, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            // generate actionId
            var actionId = new ActionId(Guid.NewGuid());

            // make sure Stuff exists in the Inbox
            StuffInfo stuffInfo;
            if (!_aggState.StuffInInbox.TryGetValue(stuffId, out stuffInfo))
            {
                throw DomainError.Named("unknown stuff", "Unknown stuff {0}", stuffId);
            }
            // TODO: May be able to use this to change the stuff description and then let that cascade down
            // as both the Project AND Action Outcome in case you wanted to use a different name than from original desc
            // With current design it may be better to kick off a "RENAME" command or Apply Renamed event for that purpose though.
            //if (stuffInfo.Description != stuffDescription)
            //{
            //    Apply(new StuffDescriptionChanged(_aggState.Id, stuffId, newDescriptionOfStuff, time.GetUtcNow()));
            //}

            // TODO: Not sure if it best to just reuse existing Events and projections (probably)
            // or if I should create a new composite event for this new command msg.
            // Thinking the former, not latter is way to go.
            Apply(new ProjectDefined(_aggState.Id, projectId, stuffInfo.Description, ProjectType.List, time));
            Apply(new ActionDefined(_aggState.Id, actionId, projectId, stuffInfo.Description, time));
            //Apply(new SingleActionProjectDefined(_aggState.Id, etc.)

            // Archive the Stuff from the Inbox now that is has transitioned from "Stuff" to a defined "Action"
            Apply(new StuffArchived(_aggState.Id, stuffId, time));
        }

        public void DefineAction(RequestId requestId, ProjectId projectId, string outcome, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();

            ProjectInfo projectInfo;
            if (!_aggState.Projects.TryGetValue(projectId, out projectInfo))
            {
                throw DomainError.Named("unknown-project", "Unknown project {0}", projectId);
            }

            var actionId = new ActionId(NewGuidIfEmpty(requestId));
            Apply(new ActionDefined(_aggState.Id, actionId, projectId, outcome , time));
        }

        ActionInfo GetActionOrThrow(ActionId id)
        {
            ActionInfo actionInfo;
            if (!_aggState.Actions.TryGetValue(id, out actionInfo))
            {
                throw DomainError.Named("unknown action", "Unknown action {0}", id);
            }
            return actionInfo;

        }

        public void MoveStuffToProject(StuffId[] stuffToMove, ProjectId projectId, ITimeProvider provider)
        {
            GetProjectOrThrow(projectId);
            var dateTime = provider.GetUtcNow();
            foreach (var stuffId in stuffToMove)
            {
                StuffInfo stuffInfo;
                if (!_aggState.StuffInInbox.TryGetValue(stuffId, out stuffInfo))
                {
                    throw DomainError.Named("unknown-stuff", "Unknown stuff {0}", stuffId);
                }

                Apply(new StuffArchived(_aggState.Id, stuffId, dateTime));
                Apply(new ActionDefined(_aggState.Id,new ActionId(Guid.NewGuid()), projectId, stuffInfo.Description, dateTime));
            }
        }

        ProjectInfo GetProjectOrThrow(ProjectId projectId)
        {
            ProjectInfo projectInfo;
            if (!_aggState.Projects.TryGetValue(projectId, out projectInfo))
            {
                throw DomainError.Named("unknown-project", "Unknown project {0}", projectId);
            }
            return projectInfo;
        }

        public void MoveActionsToProject(IEnumerable<ActionId> actions, ProjectId targetProject, DateTime timeUtc)
        {
            var project = GetProjectOrThrow(targetProject);

            foreach (var id in actions)
            {
                var action = GetActionOrThrow(id);
                if (!action.Project.Equals(targetProject))
                {

                    Apply(new ActionMovedToProject(_aggState.Id, id, action.Project, targetProject, timeUtc));
                }
            }
        }

        public void CompleteAction(ActionId actionId, ITimeProvider provider)
        {
            var actionInfo = GetActionOrThrow(actionId);
            
            if (actionInfo.Completed)
                return;// idempotency

            Apply(new ActionCompleted(_aggState.Id, 
                actionId, 
                actionInfo.Project, 
                actionInfo.Outcome, 
                provider.GetUtcNow()));

        }

        public void ChangeActionOutcome(ActionId actionId, string outcome, ITimeProvider time)
        {
            ActionInfo actionInfo;
            if (!_aggState.Actions.TryGetValue(actionId, out actionInfo))
            {
                throw DomainError.Named("unknown action", "Unknown action {0}", actionId);
            }
            if (actionInfo.Outcome != outcome)
            {
                Apply(new ActionOutcomeChanged(_aggState.Id, actionId, actionInfo.Project, outcome, time.GetUtcNow()));
            }
                
        }

        public void ChangeProjectOutcome(ProjectId projectId, string outcome, ITimeProvider time)
        {
            ProjectInfo projectInfo;
            if (!_aggState.Projects.TryGetValue(projectId, out projectInfo))
            {
                throw DomainError.Named("unknown project", "Unknown project {0}", projectId);
            }
            if (projectInfo.Outcome != outcome)
            {
                Apply(new ProjectOutcomeChanged(_aggState.Id, projectId, outcome, time.GetUtcNow()));
            }
        }

        public void ChangeProjectType(ProjectId projectId, ProjectType type, ITimeProvider time)
        {
            ProjectInfo projectInfo;
            if (!_aggState.Projects.TryGetValue(projectId, out projectInfo))
            {
                throw DomainError.Named("unknown project", "Unknown project {0}", projectId);
            }
            if (projectInfo.Type != type)
            {
                Apply(new ProjectTypeChanged(_aggState.Id, projectId, type, time.GetUtcNow()));
            }
        }

        public void ArchiveAction(ActionId actionId, ITimeProvider time)
        {
            var action = GetActionOrThrow(actionId);
            if (!action.Archived)
            {
                Apply(new ActionArchived(_aggState.Id, actionId, action.Project, time.GetUtcNow()));
            }
        }

        public void DeferActionUntil(ActionId actionId, DateTime newStartDate)
        {
            var action = GetActionOrThrow(actionId);

            if (newStartDate == action.DeferUntil)
                return;

            if (newStartDate == DateTime.MinValue)
            {
                Apply(new ActionIsNoLongerDeferred(_aggState.Id, actionId, action.DeferUntil));
                return;
            }

            if (action.DueDate != DateTime.MinValue && newStartDate > action.DueDate)
            {
                throw DomainError.Named("", "New defer date is later than due date");
            }

            if (action.DeferUntil == DateTime.MinValue)
            {
                Apply(new ActionDeferredUntil(_aggState.Id, actionId, newStartDate));
                return;
            }

            Apply(new ActionDeferDateShifted(_aggState.Id, actionId, action.DeferUntil, newStartDate));
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

            if (action.DeferUntil != DateTime.MinValue && newDueDate < action.DeferUntil)
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