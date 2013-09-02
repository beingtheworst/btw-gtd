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
        private readonly IProjectService _projectService;
        private string _projectDescription;

        public CreateNewProjectViewModel(IProjectService projectService)
        {
            _projectService = projectService;
        }

        public class NavParams
        {
            public string InitialDescription { get; set; }
        }

        // when we navigate to the a ViewModel, we have a reserved method named called 
        // "Init" and you can expect it to be called with some kind of "Nav" object
        // (which is why we created NavParams above)
        public void Init(NavParams navigationParams)
        {
            // note that the reason we don't pass the entire navigation object across
            // is because this navigation itself needs to be serializable,
            // needs to work across application restarts,
            // and you can't do that with a real object, have to do it with a key (like this Id) instead.
            ProjectDescription = navigationParams.InitialDescription;
        }

        public string ProjectDescription
        {
            get { return _projectDescription; }
            set { _projectDescription = value; RaisePropertyChanged(() => ProjectDescription); }
        }


        // Need a Command to actually Create the Project
        private MvxCommand _createProjectCommand;
        public ICommand CreateProjectCommand
        {
            get
            {
                _createProjectCommand = _createProjectCommand ?? new MvxCommand(DoCreateProject);
                return _createProjectCommand;
            }
        }

        private void DoCreateProject()
        {
            if (!Validate())
                return;

            // if Validate is happy, create a Project to be added
            // TODO: Hard coded to Trusted System 1 for now.

            var project = new Project()
            {
                TrustedSystemId = "1",
                ProjectId = Guid.NewGuid().ToString(),
                Outcome = _projectDescription
            };

            // time to store it
            _projectService.DefineProject(project);

            // probably want to close our own ViewModel after
            // the "Add Screen" has finished with its purpose, adding stuff
            // uses the Close method on the MvxNavigatingObject base class to close our MvxViewModel.
            // Close TRIES (can't guarantee it) within the platform-specific UI View layer to close
            // the current ViewModel. // TODO more on that later, depends on how you design UI.
            // Close sends a msg to the UI Layer and asks,
            // "Can you close the View that Corresponds to this ViewModel?" and makes best effort to do it.
            // On closure the previous ViewModel on nav stack should be displayed (likely InboxViewModel)
            Close(this);

        }

        // TODO: would be nice if the editor auto-validated e.g. enable/disable save button
        // call recursively?
        private bool Validate()
        {
            if (string.IsNullOrEmpty(ProjectDescription))
                return false;

            return true;
        }
    }
}
