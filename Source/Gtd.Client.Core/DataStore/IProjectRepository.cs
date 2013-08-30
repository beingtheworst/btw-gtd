using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public interface IProjectRepository
    {
        IList<Project> AllProjects();
        Project GetByProjectId(ProjectId projectId);
        void DefineProject(Project project);
    }
}
