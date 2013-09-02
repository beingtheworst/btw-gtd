using Cirrious.MvvmCross.Plugins.Messenger;

namespace Gtd.Client.Core.Services.Inbox
{
    public class InboxChangedMessage : MvxMessage
    {
        // for now we leave as a generic message.
        // in future we could add details of specifically what changed.
        public InboxChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}
