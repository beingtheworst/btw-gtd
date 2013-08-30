using System;
using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.Services.Actions;
using Gtd.Client.Core.Services.Projects;

namespace Gtd.Client.Core.ViewModels
{
    public class CreateNewProjectViewModel : MvxViewModel
    {
        // we need access to our inbox service again (TODO: put in Base class?)
        private readonly IProjectService _projectService;
        private readonly IActionService _actionService;

        public CreateNewProjectViewModel(IProjectService projectService,
                                        IActionService actionService)
        {
            _projectService = projectService;
            _actionService = actionService;

        }

        public class NewProjectParameters
        {
            public string InitialProjectDescription { get; set; }
        }

        // now we need to define our Stuff fields to Add an ItemOfStuff
        // we would like to have a ViewModel field for everything that
        // will be used in the related View

        private List<Project> _projectList;
        public List<Project> ProjectList
        {
            get { return _projectList; }
            set { _projectList = value; RaisePropertyChanged(() => ProjectList); }
        }

        private string _projectDescription;
        public string ProjectDescription
        {
            get { return _projectDescription; }
            set { _projectDescription = value; RaisePropertyChanged(() => ProjectDescription); }
        }

        private MvxCommand _newProject;
        public ICommand NewProject
        {
            get
            {
                _newProject = _newProject ?? new MvxCommand(DoNewProjectCommand);
                return _newProject;
            }
        }

        private void DoNewProjectCommand()
        {
            // do action
        }

    }
}
