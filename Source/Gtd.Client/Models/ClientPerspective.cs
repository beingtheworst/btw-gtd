using System.Collections.Generic;
using System.Collections.ObjectModel;
using Gtd.Shell.Filters;

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


        public IList<ProjectModel> ListProjects()
        {
            return CurrentModel.ProjectList;
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


        public ProjectModel GetProjectOrNull(ProjectId id)
        {
            return CurrentModel.ProjectDict[id];
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
}