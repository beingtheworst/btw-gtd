using System;
using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.Services.Actions;
using Gtd.Client.Core.Services.Inbox;
using Gtd.Client.Core.Services.Projects;

namespace Gtd.Client.Core.ViewModels
{
    public class CreateNewActionViewModel : MvxViewModel
    {
        // we need access to our inbox service again (TODO: put in Base class?)
        private readonly IInboxService _inboxService;
        private readonly IProjectService _projectService;
        private readonly IActionService _actionService;

        public CreateNewActionViewModel(IInboxService inboxService,
                                        IProjectService projectService,
                                        IActionService actionService)
        {
            _inboxService = inboxService;
            _projectService = projectService;
            _actionService = actionService;

            // Default Single Action Project to False
            _isSingleActionProject = false;
        }

        // now we need to define our Stuff fields to Add an ItemOfStuff
        // we would like to have a ViewModel field for everything that
        // will be used in the related View

        private string _actionDescription;
        public string ActionDescription
        {
            get { return _actionDescription; }
            set { _actionDescription = value; RaisePropertyChanged(() => ActionDescription); }
        }


        private List<Project> _projectList;
        public List<Project> ProjectList
        {
            get { return _projectList; }
            set { _projectList = value; RaisePropertyChanged(() => ProjectList); }
        }


        private bool _isSingleActionProject;
        public bool IsSingleActionProject
        {
            get { return _isSingleActionProject; }
            set { _isSingleActionProject = value; RaisePropertyChanged(() => IsSingleActionProject); }
        }


        private MvxCommand _newProjectCommand;
        public ICommand NewProjectCommand
        {
            get
            {
                _newProjectCommand = _newProjectCommand ?? new MvxCommand(DoNewProjectCommand);
                return _newProjectCommand;
            }
        }

        private void DoNewProjectCommand()
        {
            ShowViewModel<CreateNewProjectViewModel>
                (new CreateNewProjectViewModel
                     .NewProjectParameters() { InitialProjectDescription = _actionDescription });
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
                {
                    TrustedSystemId = "1",
                    StuffId = Guid.NewGuid().ToString(),
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

