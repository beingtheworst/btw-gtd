using System;
using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
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



        //-----
        private string _stuffDescription;
        public string StuffDescription
        {
            get { return _stuffDescription; }
            set { _stuffDescription = value; RaisePropertyChanged(() => StuffDescription); }
        }


        // Need a Command to actually Add the Stuff
        private MvxCommand _addStuffCommand;
        public ICommand AddStuffCommand
        {
            get
            { 
                _addStuffCommand = _addStuffCommand ?? new MvxCommand(DoAddStuff);
                return _addStuffCommand; 
            }
        }

        private void DoAddStuff()
        {
            if (!Validate())
                return;

            // if Validate is happy, create an ItemOfStuff to be added to the inbox
            // TODO: Hard coded to Trusted System 1 for now.
            var itemOfStuff = new ItemOfStuff()
                {  TrustedSystemId = new TrustedSystemId(1), StuffId = new StuffId(Guid.NewGuid()),
                   StuffDescription = StuffDescription,
                   TimeUtc = DateTime.UtcNow
                };

            // time to store it
            _inboxService.AddStuffToInbox(itemOfStuff);

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
            if (string.IsNullOrEmpty(StuffDescription))
                return false;

            return true;
        }
    }
}
