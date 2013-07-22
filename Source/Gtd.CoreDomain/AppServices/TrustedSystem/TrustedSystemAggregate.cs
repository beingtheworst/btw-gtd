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

        public List<Event> EventsThatCausedChange = new List<Event>();

        public TrustedSystemAggregate(TrustedSystemState aggregateStateBeforeChanges)
        {
            _aggState = aggregateStateBeforeChanges;
        }

        /// <summary> Make Aggregate realize the event happened 
        /// by applying it to the state and adding to 
        /// the list of uncommitted events</summary>
        /// <param name="newEventThatHappened"></param>
        void Apply(ITrustedSystemEvent newEventThatHappened)
        {
            // update Agg's in-memory state so if a behavior (method) has 
            // multiple steps, each subsequent step has up-to-date state to operate on
            _aggState.MakeStateRealize(newEventThatHappened);

            // update Agg's public collection of change causing Events so the 
            // AppService can use it to persist AggState as appended Events to this Agg's Event Stream
            EventsThatCausedChange.Add((Event)newEventThatHappened);
        }


        static Guid NewGuidIfEmpty(RequestId requestId)
        {
            return requestId.IsEmpty ? new RequestId(Guid.NewGuid()).Id : requestId.Id;
        }


        // Aggregate-Specific Behaviors (Methods) Below

        ActionInfo GetActionOrThrow(ActionId id)
        {
            ActionInfo info;
            if (!_aggState.Actions.TryGetValue(id, out info))
            {
                throw DomainError.Named("unknown action", "Unknown action {0}", id);
            }
            return info;

        }

        public void Create(TrustedSystemId id)
        {
            Apply(new TrustedSystemCreated(id));
        }

        public void DefineProject(RequestId requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            var defaultProjectType = ProjectType.List;
            Apply(new ProjectDefined(_aggState.Id, projectId, name, defaultProjectType, time));
        }

        public void DefineSingleActionProject(RequestId requestId, InboxStuffId inboxStuffId, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            // generate actionId
            var actionId = new ActionId(Guid.NewGuid());

            // make sure thought exists
            ThoughtInfo info;
            if (!_aggState.Thoughts.TryGetValue(inboxStuffId, out info))
            {
                throw DomainError.Named("unknown thought", "Unknown thought {0}", inboxStuffId);
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
            Apply(new ProjectDefined(_aggState.Id, projectId, info.Subject, ProjectType.List, time));
            Apply(new ActionDefined(_aggState.Id, actionId, projectId, info.Subject, time));
            //Apply(new SingleActionProjectDefined(_aggState.Id, projectId, info.Subject, ProjectType.SingleActions, actionId, info.Subject, time));

            // Maybe Archive the thought from the inbox too?
            Apply(new InboxStuffArchived(_aggState.Id, inboxStuffId, time));
        }

        public void CaptureThought(RequestId requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            //var time = provider.GetUtcNow();
            //var id = new ActionId(NewGuidIfEmpty(requestId));
            var id = new InboxStuffId(NewGuidIfEmpty(requestId));

            Apply(new InboxStuffCaptured(_aggState.Id, id, name, provider.GetUtcNow()));
        }

        public void DefineAction(RequestId requestId, ProjectId projectId, string outcome, ITimeProvider provider)
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

        public void MoveThoughtsToProject(InboxStuffId[] inboxStuffs, ProjectId projectId,
            ITimeProvider provider)
        {
            ProjectInfo p;
            if (!_aggState.Projects.TryGetValue(projectId, out p))
            {
                throw DomainError.Named("unknown-project", "Unknown project {0}", projectId);
            }
            var dateTime = provider.GetUtcNow();
            foreach (var t in inboxStuffs)
            {
                ThoughtInfo info;
                if (!_aggState.Thoughts.TryGetValue(t, out info))
                {
                    throw DomainError.Named("unknown-thought", "Unknown thought {0}", t);
                }

                
                Apply(new InboxStuffArchived(_aggState.Id, t, dateTime));
                Apply(new ActionDefined(_aggState.Id,new ActionId(Guid.NewGuid()),projectId, info.Subject,dateTime ));


            }

        }

        public void ArchiveThought(InboxStuffId inboxStuffId, ITimeProvider provider)
        {
            if (!_aggState.Inbox.Contains(inboxStuffId))
                throw DomainError.Named("no thought", "Thought {0} not found", inboxStuffId);

            Apply(new InboxStuffArchived(_aggState.Id, inboxStuffId, provider.GetUtcNow()));
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

        public void ChangeThoughtSubject(InboxStuffId inboxStuffId, string subject, ITimeProvider time)
        {
            ThoughtInfo info;
            if (!_aggState.Thoughts.TryGetValue(inboxStuffId, out info))
            {
                throw DomainError.Named("unknown thought", "Unknown thought {0}", inboxStuffId);
            }
            if (info.Subject != subject)
            {
                Apply(new NameOfInboxStuffChanged(_aggState.Id, inboxStuffId, subject, time.GetUtcNow()));
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

        public void InitIfNeeded(TrustedSystemId id)
        {
            if (_aggState.Id == null)
            {
                Create(id);
            }
        }
    }
}