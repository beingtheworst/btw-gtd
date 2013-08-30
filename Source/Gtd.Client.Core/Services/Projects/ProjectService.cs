using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.Plugins.Messenger;
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
        private readonly IMvxMessenger _mvxMessenger;

        public ProjectService(IProjectRepository projectRepository,
                              IMvxMessenger mvxMessenger)
        {
            // this wont get initialized from the Mvx IoC
            // so we do it here
            _projectRepository = projectRepository;
            _mvxMessenger = mvxMessenger;

        }

        // TODO: May want to do validation in here instead of ViewModel
        public void DefineProject(Project project)
        {
            _projectRepository.DefineProject(project);

            // send msg to tell others about new project
            // this can help properties in ViewModels stay updated

            _mvxMessenger.Publish(new ProjectsChangedMessage(this));
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

        // TODO: Add Delete functionality
        // send msg to tell others about removed project
        // this can help properties in ViewModels stay updated
        // _mvxMessenger.Publish(new ProjectsChangedMessage(this));
    }
}

