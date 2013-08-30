using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Projects
{
    public interface IProjectService
    {
        IList<Project> AllProjects();
        Project GetByProjectId(ProjectId projectId);
        void DefineProject(Project project);
    }
}
