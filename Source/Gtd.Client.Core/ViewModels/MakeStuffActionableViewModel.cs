using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.Services.Actions;
using Gtd.Client.Core.Services.Inbox;
using Gtd.Client.Core.Services.Projects;

namespace Gtd.Client.Core.ViewModels
{
    public class MakeStuffActionableViewModel : MvxViewModel
    {
        private readonly IMvxMessenger _mvxMessenger;
        private readonly IInboxService _inboxService;
        private readonly IProjectService _projectService;
        private readonly IActionService _actionService;

        private readonly MvxSubscriptionToken _projectsChangedSubToken;

        private ItemOfStuff _itemOfStuff;

        public MakeStuffActionableViewModel(IMvxMessenger mvxMessenger,
                                            IInboxService inboxService,
                                            IProjectService projectService,
                                            IActionService actionService)
        {
            _mvxMessenger = mvxMessenger;
            _inboxService = inboxService;
            _projectService = projectService;
            _actionService = actionService;

            ReloadProjects();

            // subscribe to Projects Changed messages to react to changes in project service
            _projectsChangedSubToken =
                mvxMessenger.Subscribe<ProjectsChangedMessage>(OnProjectsChanged);
        }

        void OnProjectsChanged(ProjectsChangedMessage message)
        {
            ReloadProjects();
        }

        public class NavParams
        {
            public string StuffId { get; set; }
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
            ItemOfStuff = _inboxService.GetByStuffId(navigationParams.StuffId);
        }


        // in this ViewModel we want access to the ItemOfStuff that was 
        // passed in from view via a property
        // will use RaisePropertyChanged to refresh it as needed
        public ItemOfStuff ItemOfStuff
        {
            get { return _itemOfStuff; }
            set { _itemOfStuff = value; RaisePropertyChanged(() => ItemOfStuff); }
        }

        // I want to be able to delete or archive stuff out of inbox if it becomes actionable
        private MvxCommand _trashStuffCommand;
        public ICommand TrashStuffCommand
        {
            get
            { 
                _trashStuffCommand = _trashStuffCommand ?? new MvxCommand(DoTrashStuffCommand);
                return _trashStuffCommand; 
            }
        }

        private void DoTrashStuffCommand()
        {
            _inboxService.TrashStuff(ItemOfStuff);
            
            // if you just trashed the stuff you started with then we are done
            // go back to precious screen
            Close(this);
        }


        // TODO: This feature is supported in the domain, but not on the client rigth now
        //private MvxCommand _archiveStuffFromInbox;
        //public ICommand ArchiveStuffFromInbox
        //{
        //    get
        //    {
        //        _archiveStuffFromInbox = _archiveStuffFromInbox ?? new MvxCommand(DoArchiveStuffCommand);
        //        return _archiveStuffFromInbox; 
        //    }
        //}

        //private void DoArchiveStuffCommand()
        //{
        //    // do action
        //}


        // i need a list of the current projects

        private IList<Project> _projectList;
        public IList<Project> ProjectList
        {
            get { return _projectList; }
            set { _projectList = value; RaisePropertyChanged(() => ProjectList); }
        }
     
        void ReloadProjects()
        {
            if (_projectService.AllProjects().Count > 0)
            {
                ProjectList = _projectService.AllProjects();
            }
        }

        
        // I want to be able to add items to the store of projects and actions
        

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
                (new CreateNewProjectViewModel.NavParams() {StuffDescription = "hi"});
        }




        // need to know if user says this is a single action project so I can just create
        private bool _isSingleActionProject;
        public bool IsSingleActionProject
        {
            get { return _isSingleActionProject; }
            set { _isSingleActionProject = value; RaisePropertyChanged(() => IsSingleActionProject); }
        }


        // an entry in the project and actions systems with same info??

        // need a place to enter this on screen
        // What’s the next physical/visible action to take on this project?

        private string _nextActionIs;
        public string NextActionIs
        {
            get { return _nextActionIs; }
            set { _nextActionIs = value; RaisePropertyChanged(() => NextActionIs); }
        }

        // need AddNextAction cmd
        private MvxCommand _addNextAction;
        public ICommand AddNextAction
        {
            get
            { 
                _addNextAction = _addNextAction ?? new MvxCommand(DoAddNextActionCommand);
                return _addNextAction; 
            }
        }

        private void DoAddNextActionCommand()
        {
            // do action
        }
    }
}
