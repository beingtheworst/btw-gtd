using Gtd.Client.Controllers;
using Gtd.Client.Views.AddStuffToInbox;
using Gtd.Client.Views.CaptureThought;
using Gtd.Client.Views.Navigation;
using Gtd.Client.Views.Project;

namespace Gtd.Client
{
    public class UserInterface {
        public UserInterface() {}

        public UserInterface(MainForm form, IAddStuffToInboxWizard addStuffToInboxWizard, IAddActionToProjectWizard addActionToProjectWizard, IDefineProjectWizard defineProjectWizard, IInboxView inboxView, INavigationView navigation, IProjectView projectView)
        {
            Menu = form;
            Log = form;
            BackView = form;
            AddStuffToInboxWizard = addStuffToInboxWizard;
            AddActionToProjectWizard = addActionToProjectWizard;
            DefineProjectWizard = defineProjectWizard;
            InboxView = inboxView;
            Navigation = navigation;
            ProjectView = projectView;
        }

        public IMainMenu Menu { get;  set; }
        public ILogView Log { get;  set; }
        public INavigateBackView BackView { get;  set; }

        public IAddStuffToInboxWizard AddStuffToInboxWizard { get;  set; }

        public IAddActionToProjectWizard AddActionToProjectWizard { get;  set; }

        public IDefineProjectWizard DefineProjectWizard { get;  set; }

        public IInboxView InboxView { get;  set; }

        public INavigationView Navigation { get;  set; }

        public IProjectView ProjectView { get;  set; }
    }
}