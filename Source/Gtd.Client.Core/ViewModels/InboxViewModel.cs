using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.Services.Inbox;

namespace Gtd.Client.Core.ViewModels
{
    public class InboxViewModel : MvxViewModel
    {
        private readonly IInboxService _inboxService;
        private readonly IMvxMessenger _mvxMessenger;

        private readonly MvxSubscriptionToken _inboxChangedSubToken;

        public InboxViewModel(IInboxService inboxService,
                              IMvxMessenger mvxMessenger)
        {
            _inboxService = inboxService;
            _mvxMessenger = mvxMessenger;

            // fill our Inbox up with Items of Stuff from IInboxService
            ReloadInbox();

            // subscribe to Inbox Changed messages to react to changes in inbox service
            _inboxChangedSubToken =
                mvxMessenger.Subscribe<InboxChangedMessage>(OnInboxChanged);
        }

        void OnInboxChanged(InboxChangedMessage message)
        {
            ReloadInbox();
        }

        void ReloadInbox()
        {
            if (_inboxService.AllStuffInInbox().Count > 0)
            {
                StuffInInbox = _inboxService.AllStuffInInbox();
            }
            else
            {
                var noStuff = new ItemOfStuff 
                    {StuffDescription = "no stuff in your inbox. \n  + add some stuff now!"};

                var noStuffList = new List<ItemOfStuff> {noStuff};

                StuffInInbox = noStuffList;
            }
        }

        private IList<ItemOfStuff> _stuffInInbox;
        public IList<ItemOfStuff> StuffInInbox
        {
            get { return _stuffInInbox; }
            set { _stuffInInbox = value; RaisePropertyChanged(() => StuffInInbox); }
        }

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
            ShowViewModel<AddStuffViewModel>();
        }

        //private MvxCommand _moveStuffToProject;
        //public ICommand MoveStuffToProject
        //{
        //    get
        //    {
        //        _moveStuffToProject = _moveStuffToProject ?? new MvxCommand(DoMoveStuffToProjectCommand);
        //        return _moveStuffToProject;
        //    }
        //}

        //private void DoMoveStuffToProjectCommand()
        //{
        //    ShowViewModel<CreateNewProjectViewModel>
        //        (new CreateNewProjectViewModel
        //          .NewProjectParameters() { InitialProjectDescription = "TODO: Temp Selected Inbox Item Text" });
        //}

        public ICommand MakeStuffActionableCommand
        {
            get
            { 
                return new MvxCommand<ItemOfStuff>(item =>
                           ShowViewModel<MakeStuffActionableViewModel>
                           (new MakeStuffActionableViewModel.NavParams() 
                           { StuffId = item.StuffId }));
            }
        }
    }
}
