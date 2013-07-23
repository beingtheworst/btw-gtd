using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public readonly ImmutableList<ImmutableStuff> Stuff;

        public ImmutableInbox(ImmutableList<ImmutableStuff> stuff)
        {
            Stuff = stuff;
        }
    }

    public sealed class ImmutableClientModel
    {
        public readonly ImmutableList<ImmutableStuff> Inbox;
        public readonly ImmutableList<ImmutableProject> Projects;

        public ImmutableClientModel(ImmutableList<ImmutableStuff> inbox, ImmutableList<ImmutableProject> projects)
        {
            Inbox = inbox;
            Projects = projects;
        }
    }

    public sealed class ImmutableStuff
    {
        public readonly StuffId StuffId;
        public readonly string Description;
        public readonly string UIKey;

        public ImmutableStuff WithDescription(string descriptionOfStuff)
        {
            return new ImmutableStuff(StuffId, descriptionOfStuff, UIKey);
        }

        public ImmutableStuff(StuffId stuffId, string descriptionOfStuff, string uiKey)
        {
            StuffId = stuffId;
            Description = descriptionOfStuff;
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