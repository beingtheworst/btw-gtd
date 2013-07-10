using System.Collections.Generic;

namespace Gtd.Client.Models
{
    public sealed class ClientContext 
    {
        public ClientModel CurrentModel { get; private set; }

        public void SwitchToModel(ClientModel model)
        {
            CurrentModel = model;
        }


        public IList<ProjectView> ListProjects()
        {
            return CurrentModel.ProjectList;
        }

        public ThoughtView[] ListInbox()
        {
            return CurrentModel.Thoughts.ToArray();
        }

        public ProjectView GetProjectOrNull(ProjectId id)
        {
            return CurrentModel.ProjectDict[id];
        }
    }
}