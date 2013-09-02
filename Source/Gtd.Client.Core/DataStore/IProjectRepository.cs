using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public interface IProjectRepository
    {
        // SQLite doesn't like my custom Id types. Goign to string for now.

        IList<Project> AllProjects();
        //Project GetByProjectId(ProjectId projectId);
        Project GetByProjectId(string projectId);
        void DefineProject(Project project);
    }
}
