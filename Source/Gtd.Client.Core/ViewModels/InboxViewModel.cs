using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.ViewModels
{
    public class InboxViewModel : MvxViewModel
    {
        private IList<ItemOfStuff> _stuffInInbox;
        public IList<ItemOfStuff> StuffInInbox
        {
            get { return _stuffInInbox; }
            set { _stuffInInbox = value; RaisePropertyChanged(() => StuffInInbox); }
        }
    }
}
