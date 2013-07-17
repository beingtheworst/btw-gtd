using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gtd.Shell.Filters;
using System.Linq;

namespace Gtd.Client.Models
{
    public sealed class ClientPerspective 
    {
        public ClientModel CurrentModel { get; private set; }
        public IFilterCriteria CurrentFilter { get; private set; }

        public ClientPerspective()
        {
            CurrentFilter = new AllActionsFilter();
        }

        public void SwitchToModel(ClientModel model)
        {
            CurrentModel = model;
        }


        public IList<ImmutableProject> ListProjects()
        {
            return CurrentModel.ListProjects();
        }

        public ImmutableInbox GetInbox()
        {
            // TODO: adjust by filters
            return CurrentModel.GetInbox();
        }

        public int GetNumberOfThoughtsInInbox()
        {
            // TODO: adjust by filters
            return CurrentModel.GetNumberOfThoughtsInInbox();
        }


        public ImmutableProject GetProjectOrNull(ProjectId id)
        {

            return CurrentModel.GetProjectOrNull(id);
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
        public readonly ActionId Id;
        public readonly string Outcome;
        public readonly bool Completed;
        public readonly bool Archived;
        public readonly ProjectId ProjectId;
        public readonly DateTime StartDate;
        public readonly DateTime DueDate;
        public readonly string UIKey;

        public ImmutableAction(string uiKey, ActionId id, string outcome, bool completed, bool archived, ProjectId projectId, DateTime startDate, DateTime dueDate)
        {
            UIKey = uiKey;
            Id = id;
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

}