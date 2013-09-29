using System;
using System.IO;
using System.Windows.Forms;
using Btw.Portable;
using Gtd.Client.Controllers;
using Gtd.Client.Models;
using Gtd.Client.Views.AddStuffToInbox;
using Gtd.Client.Views.CaptureThought;
using Gtd.Client.Views.Navigation;
using Gtd.Client.Views.Project;
using Gtd.Shell;

namespace Gtd.Client
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // this Client App uses a standard "Windows Forms" application as its host
            // this "Application" code is standard Windows Forms stuff
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            

            var form = new MainForm();

            var navigation = new NavigationView();
            form.NavigationRegion.RegisterDock(navigation, "nav");
            form.NavigationRegion.SwitchTo("nav");

            var projectView = new ProjectView();
            projectView.AttachTo(form.MainRegion);

            var inboxView = new InboxView();
            inboxView.AttachTo(form.MainRegion);
            var addStuffToInboxWizard = new AddStuffToInboxForm(form);
            var addActionToProjectWizard = new AddActionToProjectForm(form);
            var defineProjectWizard = new DefineProjectForm(form);


            var ui = new UserInterface
                {
                    InboxView = inboxView,
                    ProjectView = projectView,
                    BackView = form,
                    Log = form,
                    AddActionToProjectWizard = addActionToProjectWizard,
                    AddStuffToInboxWizard = addStuffToInboxWizard,
                    DefineProjectWizard = defineProjectWizard,
                    Menu = form,
                    Navigation = navigation
                };
            

            Bootstrap.WireControlLogic(ui);

            Application.Run(form);
        }
    }
}
