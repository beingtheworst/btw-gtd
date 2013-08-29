using System.Collections.Generic;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;
using Gtd.Client.Core.Services.Inbox;

namespace Gtd.Client.Core.ViewModels
{
    public class InboxViewModel : MvxViewModel
    {
        readonly IInboxService _inboxService;

        public InboxViewModel(IInboxService inboxService)
        {
            _inboxService = inboxService;

            // fill our Inbox up with Items of Stuff from IInboxService
            ReloadInbox();
        }

        void ReloadInbox()
        {
            StuffInInbox = _inboxService.AllStuffInInbox();
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
                _addStuffCommand = _addStuffCommand ?? new MvxCommand(DoAdd);
                return _addStuffCommand;
            }
        }

        private void DoAdd()
        {
            ShowViewModel<AddStuffViewModel>();
        }
    }
}
