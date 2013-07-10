using System.Collections.Generic;

namespace Gtd.Client.Models
{
    public sealed class ClientModelProvider 
    {
        public ClientModel Model { get; private set; }

        public void SwitchToModel(ClientModel model)
        {
            Model = model;
        }


        public IList<ProjectView> ListProjects()
        {
            return Model.ProjectList;
        }

        public ThoughtView[] ListInbox()
        {
            return Model.Thoughts.ToArray();
        }

        public ProjectView GetProjectOrNull(ProjectId id)
        {
            return Model.ProjectDict[id];
        }
    }
}