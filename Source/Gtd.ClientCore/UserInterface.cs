using Gtd.Client.Controllers;
using Gtd.Client.Views.AddStuffToInbox;
using Gtd.Client.Views.CaptureThought;
using Gtd.Client.Views.Navigation;
using Gtd.ClientCore.Controllers;

namespace Gtd.Client
{
    public class UserInterface
    {
        public IMainMenu Menu { get; set; }
        public ILogView Log { get; set; }
        public INavigateBackView BackView { get; set; }
        public IAddStuffToInboxWizard AddStuffToInboxWizard { get; set; }
        public IAddActionToProjectWizard AddActionToProjectWizard { get; set; }
        public IDefineProjectWizard DefineProjectWizard { get; set; }
        public IInboxView InboxView { get; set; }
        public INavigationView Navigation { get; set; }
        public IProjectView ProjectView { get; set; }
    }
}