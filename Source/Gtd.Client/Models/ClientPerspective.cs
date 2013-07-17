using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gtd.Shell.Filters;
using System.Linq;

namespace Gtd.Client.Models
{
    public sealed class ClientPerspective 
    {
        public ClientModel Model { get; private set; }
        public IFilterCriteria CurrentFilter { get; private set; }

        public ClientPerspective()
        {
            CurrentFilter = new AllActionsFilter();
        }

        public void SwitchToModel(ClientModel model)
        {
            Model = model;
        }


        public IList<ImmutableProject> ListProjects()
        {
            return Model.ListProjects();
        }

       


        

        public FilteredProject GetProject(ProjectId id)
        {
            var pid = Model.GetProjectOrNull(id);

            var actions = CurrentFilter.FilterActions(pid).ToList().AsReadOnly();
            var count = CurrentFilter.FormatActionCount(actions.Count);
            return new FilteredProject(pid.ProjectId, pid.Outcome, pid.Type, pid.UIKey, actions, count);
        }


        public void SwitchToFilter(IFilterCriteria criteria)
        {
            this.CurrentFilter = criteria;
        }
    }

    public sealed class ImmutableInbox
    {
        public readonly ReadOnlyCollection<ImmutableThought> Thoughts;

        public ImmutableInbox(ReadOnlyCollection<ImmutableThought> thoughts)
        {
            Thoughts = thoughts;
        }
    }

    public sealed class ImmutableThought
    {
        public readonly ThoughtId ThoughtId;
        public readonly string Subject;
        public readonly string UIKey;

        public ImmutableThought(ThoughtId thoughtId, string subject, string uiKey)
        {
            ThoughtId = thoughtId;
            Subject = subject;
            UIKey = uiKey;
        }
    }

    public sealed class ImmutableAction
    {
        public readonly ActionId ActionId;
        public readonly string Outcome;
        public readonly bool Completed;
        public readonly bool Archived;
        public readonly ProjectId ProjectId;
        public readonly DateTime StartDate;
        public readonly DateTime DueDate;
        public readonly string UIKey;

        public ImmutableAction(string uiKey, ActionId actionId, string outcome, bool completed, bool archived, ProjectId projectId, DateTime startDate, DateTime dueDate)
        {
            UIKey = uiKey;
            ActionId = actionId;
            Outcome = outcome;
            Completed = completed;
            Archived = archived;
            ProjectId = projectId;
            StartDate = startDate;
            DueDate = dueDate;
        }
    }

    public sealed class ImmutableProject
    {
        public readonly ProjectId ProjectId;
        public readonly string Outcome;
        public readonly ProjectType Type;
        public readonly string UIKey;
        public readonly ReadOnlyCollection<ImmutableAction> Actions; 

        public ImmutableProject(string uiKey, ProjectId projectId, string outcome, ProjectType type, ReadOnlyCollection<ImmutableAction> actions)
        {
            UIKey = uiKey;
            ProjectId = projectId;
            Outcome = outcome;
            Type = type;
            Actions = actions;
        }
    }

    public sealed class FilteredProject
    {
        public readonly ProjectId ProjectId;
        public readonly string Outcome;
        public readonly ProjectType Type;
        public readonly string UIKey;
        public readonly ReadOnlyCollection<ImmutableAction> FilteredActions;
        public readonly string ActionCount;

        public FilteredProject(ProjectId projectId, string outcome, ProjectType type, string uiKey, ReadOnlyCollection<ImmutableAction> filteredActions, string actionCount)
        {
            ProjectId = projectId;
            Outcome = outcome;
            Type = type;
            UIKey = uiKey;
            FilteredActions = filteredActions;
            ActionCount = actionCount;
        }
    }
}