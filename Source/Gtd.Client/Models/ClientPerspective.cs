using System.Collections.Generic;
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


        public IList<ProjectView> ListProjects()
        {
            return CurrentModel.ProjectList;
        }

        public IList<ThoughtView> ListInbox()
        {
            var thoughts = CurrentModel.Thoughts;
            
            return thoughts.ToArray();
        }


        public ProjectView GetProjectOrNull(ProjectId id)
        {
            return CurrentModel.ProjectDict[id];
        }

        public void SwitchToFilter(IFilterCriteria criteria)
        {
            this.CurrentFilter = criteria;
        }
    }
}