using System;
using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.Tenant
{
    public sealed class TenantState : ITenantState
    {
        public static TenantState BuildStateFromEventHistory(IEnumerable<Event> events)
        {
            var state = new TenantState();
            foreach (var e in events)
            {
                state.MakeStateRealizeThat((ITenantEvent) e);
            }
            return state;
        }

        public TenantState()
        {
            Inbox = new HashSet<Guid>();
        }

        public void MakeStateRealizeThat(ITenantEvent thisEventTypeHappened)
        {
            #region What Is This Code/Syntax Doing?
            // After the Aggregate records the Event, we announce this Event to all those
            // that care about it to help them Realize that things have changed.
            // We are telling the compiler to call one of the "When" methods defined above to achieve this realization.
            // The "dynamic" syntax below is just a shortcut we are using so we don't
            // have to create a large if/else block for a bunch of type-specific static Event types.
            // This shortcut using the "dynamic" keyword syntax means:
            // "Call 'this' Aggregates's instance of the 'When' method
            // that has a method signature of:
            // When(WhateverTheCurrentEventTypeIsOf-thisEventTypeHappened)".
            #endregion

            ((dynamic)this).When((dynamic)thisEventTypeHappened);
        }

        public TenantId Id { get; private set; }

        public HashSet<Guid> Inbox { get; private set; } 
        public void When(TenantCreated e)
        {
            Id = e.Id;
        }

        public void When(ThoughtCaptured e)
        {
            Inbox.Add(e.ThoughtId);
        }

        public void When(ThoughtArchived e)
        {
            Inbox.Remove(e.ThoughtId);
        }

        public void When(ActionDefined e)
        {
            Actions.Add(e.Action, new ActionInfo(e.Action, e.ActionName));
        }

        public void When(ProjectDefined e)
        {
            Projects.Add(e.Project, new ProjectInfo(e.Project, e.ProjectOutcome));
        }

        public void When(ActionAssignedToProject e)
        {
            var action = Actions[e.Action];
            var project = Projects[e.NewProject];

            action.LinkToProject(project.Id);
            project.AddAction(action.Id);
        }

        public void When(ActionRemovedFromProject e)
        {
            Actions[e.Action].RemoveFromProject(e.OldProject);
            Projects[e.OldProject].RemoveAction(e.Action);
        }

        public void When(ActionMovedToProject e)
        {
            var action = Actions[e.Action];
            var oldProject = Projects[e.OldProject];
            var newProject = Projects[e.NewProject];

            action.MoveToProject(e.OldProject, e.NewProject);
            
            newProject.AddAction(action.Id);
            oldProject.RemoveAction(action.Id);
        }

        public void When(ActionRemoved e)
        {
            Actions[e.Action].EnsureCleanRemoval();
            Actions.Remove(e.Action);
        }

        public void When(ActionRenamed e)
        {
            Actions[e.Action].Rename(e.Name);
        }

        public void When(ActionCompleted e)
        {
            Actions[e.Action].MarkAsCompleted();
        }


        public readonly IDictionary<ActionId, ActionInfo> Actions = new Dictionary<ActionId, ActionInfo>(); 
        public readonly IDictionary<ProjectId, ProjectInfo> Projects = new Dictionary<ProjectId, ProjectInfo>(); 
    }

    /// <summary>
    /// Action entity for the aggregate state. It maintains only invariants within itself.
    /// Invariants between entities are maintained by the state
    /// </summary>
    public sealed class ActionInfo // VO
    {
        public ActionId Id { get; private set; }
        public string Name { get; private set; }

        public ProjectId Project { get; private set; }

        public bool HasProject { get { return !Project.IsEmpty; } }

        public bool Completed { get; private set; }

        public ActionInfo(ActionId id, string name)
        {
            Id = id;
            Name = name;
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

        public void Rename(string newName)
        {
            Name = newName;
        }
        public void EnsureCleanRemoval()
        {
            if (HasProject)
                throw new InvalidOperationException("Can't remove action that is still assigned to project");
        }
    }


    public sealed class ProjectInfo
    {
        public ProjectId Id { get; private set; }
        public string Name { get; private set; }

        readonly List<ActionId> _actions = new List<ActionId>();
         

        public ProjectInfo(ProjectId id, string name)
        {
            Id = id;
            Name = name;
        }

        public void Rename(string newName)
        {
            Name = newName;
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


    
}