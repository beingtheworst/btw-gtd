using System;
using System.Collections.Generic;
using Gtd.Client.Core.DataStore;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Projects
{
    // We use this service so we can have singletons holding all
    // the stuff in the inbox in memory so that the ViewModels are not
    // constantly hitting the database layer to get it

    // probably going to be a Lazy Singleton per config convention in App.cs "Service"
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            // this wont get initialized from the Mvx IoC
            // so we do it here
            _projectRepository = projectRepository;

        }

        // TODO: May want to do validation in here instead of ViewModel
        public void DefineProject(Project project)
        {
            _projectRepository.DefineProject(project);
        }

        public IList<Project> AllProjects()
        {
            // this will likely get cached down the road but use All for now
            return _projectRepository.AllProjects();
        }

        public Project GetByProjectId(string projectId)
        {
            return _projectRepository.GetByProjectId(projectId);
        }
    }
}

