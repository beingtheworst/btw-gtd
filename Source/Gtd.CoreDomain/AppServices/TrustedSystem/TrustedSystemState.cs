using System;
using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.TrustedSystem
{
    public sealed class TrustedSystemState : ITrustedSystemState
    {
        public TrustedSystemState()
        {
        }

        public TrustedSystemId Id { get; private set; }

        public static TrustedSystemState BuildStateFromEventHistory(IEnumerable<Event> events)
        {
            var aggState = new TrustedSystemState();

            foreach (var eventThatHappened in events)
            {
                aggState.MakeStateRealize((ITrustedSystemEvent) eventThatHappened);
            }
            return aggState;
        }

        public void MakeStateRealize(ITrustedSystemEvent thisEventTypeHappened)
        {
            #region What Is This Code/Syntax Doing?
            // Announce that a specific Type of Event Message occured
            // so that this AggregateState instance can Realize it and change its state because of it.
            // We are telling the compiler to call one of the "When" methods in this class to achieve this realization.
            // The "dynamic" syntax below is just a shortcut we are using so we don't
            // have to create a large if/else block for a bunch of type-specific static Event types.
            // This shortcut using the "dynamic" keyword syntax means:
            // "Call the 'When' method of 'this' AggregateState instance
            // that has a method signature of:
            // When(WhateverTheEventTypeIsOf-thisEventTypeHappened)".
            #endregion

            ((dynamic)this).When((dynamic)thisEventTypeHappened);
        }


        // When methods reacting to Events to change in memory state of the Aggregate

        public void When(TrustedSystemCreated evnt)
        {
            Id = evnt.Id;
        }

        public void When(ThoughtCaptured evnt)
        {
            var info = new ThoughtInfo(evnt.ThoughtId, evnt.Thought);
            Thoughts.Add(evnt.ThoughtId, info);
            Inbox.Add(evnt.ThoughtId);
        }

        public void When(ThoughtArchived evnt)
        {
            Inbox.Remove(evnt.ThoughtId);
        }

        public void When(ActionDefined evnt)
        {
            Actions.Add(evnt.ActionId, new ActionInfo(evnt.ActionId, evnt.Outcome));
        }

        public void When(ProjectDefined evnt)
        {
            Projects.Add(evnt.ProjectId, new ProjectInfo(evnt.ProjectId, evnt.ProjectOutcome, evnt.Type));
        }

        public void When(ProjectTypeChanged e)
        {
            Projects[e.ProjectId].ChangeType(e.Type);
        }

        public void When(ActionAssignedToProject evnt)
        {
            var action = Actions[evnt.ActionId];
            var project = Projects[evnt.NewProject];

            action.LinkToProject(project.Id);
            project.AddAction(action.Id);
        }

        public void When(ActionRemovedFromProject evnt)
        {
            Actions[evnt.ActionId].RemoveFromProject(evnt.OldProject);
            Projects[evnt.OldProject].RemoveAction(evnt.ActionId);
        }

        public void When(ActionMovedToProject evnt)
        {
            var action = Actions[evnt.ActionId];
            var oldProject = Projects[evnt.OldProject];
            var newProject = Projects[evnt.NewProject];

            action.MoveToProject(evnt.OldProject, evnt.NewProject);
            
            newProject.AddAction(action.Id);
            oldProject.RemoveAction(action.Id);
        }

        public void When(ActionArchived evnt)
        {
            Actions[evnt.ActionId].MakeArchived();
        }

        public void When(ActionOutcomeChanged evnt)
        {
            Actions[evnt.ActionId].ChangeOutcome(evnt.ActionOutcome);
        }

        public void When(ProjectOutcomeChanged evnt)
        {
            Projects[evnt.ProjectId].ChangeOutcome(evnt.ProjectOutcome);
        }

        public void When(ThoughtSubjectChanged e)
        {
            Thoughts[e.ThoughtId].ChangeSubject(e.Subject);
        }

        public void When(StartDateAssignedToAction e)
        {
            Actions[e.ActionId].StartDateAssigned(e.NewStartDate);
        }

        public void When(ActionStartDateMoved e)
        {
            Actions[e.ActionId].StartDateMoved(e.OldstartDate, e.NewStartDate);
        }

        public void When(StartDateRemovedFromAction e)
        {
            Actions[e.ActionId].StartDateRemoved(e.OldStartDate);
        }

        public void When(DueDateAssignedToAction e)
        {
            Actions[e.ActionId].DueDateAssigned(e.NewDueDate);
        }

        public void When(ActionDueDateMoved e)
        {
            Actions[e.ActionId].DueDateMoved(e.OldDueDate, e.NewDueDate);
        }

        public void When(DueDateRemovedFromAction e)
        {
            Actions[e.ActionId].DueDateRemoved(e.OldDueDate);
        }

        public void When(ActionCompleted evnt)
        {
            Actions[evnt.ActionId].MarkAsCompleted();
        }


        public readonly IDictionary<ActionId, ActionInfo> Actions = new Dictionary<ActionId, ActionInfo>(); 
        public readonly IDictionary<ProjectId, ProjectInfo> Projects = new Dictionary<ProjectId, ProjectInfo>(); 
        public readonly IDictionary<Guid, ThoughtInfo> Thoughts = new Dictionary<Guid, ThoughtInfo>(); 
        public readonly HashSet<Guid> Inbox = new HashSet<Guid>(); 
    }

    /// <summary>
    /// These Value Objects maintain only invariants within themselves.
    /// Invariants between entities are maintained by the state.
    /// </summary>
    public sealed class ActionInfo // VO
    {
        public ActionId Id { get; private set; }
        public string Outcome { get; private set; }
        public ProjectId Project { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime DueDate { get; private set; }

        public bool HasProject { get { return !Project.IsEmpty; } }

        public bool Completed { get; private set; }
        public bool Archived { get; private set; }

        public ActionInfo(ActionId id, string name)
        {
            Id = id;
            Outcome = name;
        }

        public void MakeArchived()
        {
            Archived = true;
        }

        public void LinkToProject(ProjectId id)
        {
            if (HasProject)
                throw new InvalidOperationException("Action already assigned to a project");
            Project = id;

        }

        public void MarkAsCompleted()
        {
            if (Completed == true)
                throw new InvalidOperationException("Action is already completed");
            Completed = true;
        }

        public void MoveToProject(ProjectId oldProject, ProjectId newProject)
        {
            if (oldProject != Project)
                throw new InvalidOperationException("OldProject != CurrentProject");
            if (newProject == ProjectId.Empty)
                throw new InvalidOperationException("NewProject == Empty");
            Project = newProject;
        }
        public void RemoveFromProject(ProjectId project)
        {
            if (project != Project)
                throw new InvalidOperationException("OldProject != CurrentProject");
            Project = ProjectId.Empty;
        }

        public void ChangeOutcome(string newName)
        {
            Outcome = newName;
        }
      

        public void EnsureCleanRemoval()
        {
            if (HasProject)
                throw new InvalidOperationException("Can't remove action that is still assigned to project");
        }


        public void StartDateMoved(DateTime oldTime, DateTime newTime)
        {
            if (StartDate != oldTime)
                throw new ArgumentException("Expected old time matching to actual date");
            StartDate = newTime;
        }

        public void StartDateAssigned(DateTime startDate)
        {
            if (StartDate != DateTime.MinValue)
                throw new ArgumentException("Expected null date");
            StartDate = startDate;
        }

        public void StartDateRemoved(DateTime startDate)
        {
            if (StartDate != startDate)
                throw new ArgumentException("Expected old time matching to actual date");
            StartDate = DateTime.MinValue;
        }

        public void DueDateRemoved(DateTime dueDate)
        {
            if (DueDate != dueDate)
                throw new ArgumentException("Expected old time matching to actual date");
            DueDate = DateTime.MinValue;
        }

        public void DueDateMoved(DateTime oldTime, DateTime newTime)
        {
            if (DueDate != oldTime)
                throw new ArgumentException("Expected old time matching to actual date");
            DueDate = newTime;
        }

        public void DueDateAssigned(DateTime dueDate)
        {
            if (DueDate != DateTime.MinValue)
                throw new ArgumentException("Expected null date");
            DueDate = dueDate;
        }
    }

    public sealed class ProjectInfo
    {
        public ProjectId Id { get; private set; }
        public string Outcome { get; private set; }
        public ProjectType Type { get; private set; }


        readonly List<ActionId> _actions = new List<ActionId>();
         

        public ProjectInfo(ProjectId id, string name, ProjectType type)
        {
            Enforce.NotEmpty(name,"name");

            Id = id;
            Outcome = name;
            Type = type;
        }

        public void ChangeType(ProjectType type)
        {
            Type = type;
        }

        public void ChangeOutcome(string newName)
        {
            Outcome = newName;
        }

        public void AddAction(ActionId action)
        {
            _actions.Add(action);
        }
        public void RemoveAction(ActionId action)
        {
            _actions.Remove(action);
        }
    }

    public sealed class ThoughtInfo
    {
        public Guid Id { get; private set; }
        public string Subject { get; private set; }

        public ThoughtInfo(Guid id, string subject)
        {
            Enforce.NotEmpty(id, "id");
            Enforce.NotEmpty(subject, "subject");

            Id = id;
            Subject = subject;
        }

        public void ChangeSubject(string subject)
        {
            Enforce.NotEmpty(subject, "subject");

            Subject = subject;
        }
    }
}