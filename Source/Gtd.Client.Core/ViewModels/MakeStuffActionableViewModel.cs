using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.Services.Actions;
using Gtd.Client.Core.Services.Inbox;
using Gtd.Client.Core.Services.Projects;

namespace Gtd.Client.Core.ViewModels
{
    public class MakeStuffActionableViewModel : MvxViewModel
    {
        private readonly IInboxService _inboxService;
        private readonly IProjectService _projectService;
        private readonly IActionService _actionService;

        private ItemOfStuff _itemOfStuff;

        public MakeStuffActionableViewModel(IInboxService inboxService,
                                            IProjectService projectService,
                                            IActionService actionService)
        {
            _inboxService = inboxService;
            _projectService = projectService;
            _actionService = actionService;
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

        
        // I want to be able to add items to the store of projects and actions
        // i need a list of the current projects
        // need to be able to define a new project
        // need to know if user says this is a single action project so I can just create
        // an entry in the project and actions systems with same info??

        // need a place to enter this on screen
        // What’s the next physical/visible action to take on this project?

        // need AddNextAction cmd
    }
}
