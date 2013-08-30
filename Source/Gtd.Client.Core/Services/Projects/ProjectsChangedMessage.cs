using Cirrious.MvvmCross.Plugins.Messenger;

namespace Gtd.Client.Core.Services.Projects
{
    public class ProjectsChangedMessage : MvxMessage
    {
        // for now we leave as a generic message.
        // in future we could add details of specifically what changed.
        public ProjectsChangedMessage(object sender)
            : base(sender)
        {
        }
    }
}

