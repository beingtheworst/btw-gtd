using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Btw.Portable;
using Gtd.Client.Models;
using Gtd.Client.Views.Actions;
using Gtd.Client.Views.CaptureThought;

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
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var bus = new InMemoryBus("UI");

            var file = new FileAppendOnlyStore(new DirectoryInfo(Directory.GetCurrentDirectory()));
            file.Initialize();


            var messageStore = new MessageStore(file);
            messageStore.LoadDataContractsFromAssemblyOf(typeof(ActionDefined));

            var events = new AppEventStore(messageStore);

            var controller = new AppController(bus, events);

            
            var mainQueue = new QueuedHandler(controller, "Main Queue");

            controller.SetMainQueue(mainQueue);
            events.SetDispatcher(mainQueue);

            var provider = new ClientPerspective();

            ClientPerspectiveController.WireTo(events, provider, bus, mainQueue);
            
            // create services and bind them to the bus

            // we wire all controls together in a native way.
            // then we add adapters on top of that

            var form = new MainForm();

            var filterService = new FilterService();

            MainFormController.Wire(form, mainQueue, bus,filterService);
            InboxController.Wire(form.MainRegion, mainQueue, bus, provider);
            NavigationController.Wire(form.NavigationRegion, mainQueue, bus, provider);
            ProjectController.Wire(form.MainRegion, mainQueue, bus, provider);
            LogController.Wire(form, bus);

            CaptureThoughtController.Wire(new CaptureThoughtForm(form), bus, mainQueue);
            DefineProjectController.Wire(new DefineProjectForm(form),bus, mainQueue);

            mainQueue.Enqueue(new AppInit());
            mainQueue.Start();
            
            Application.Run(form);
        }
    }


    public sealed class Region 
    {
        readonly Control _container;
        readonly IDictionary<string,Control> _controls= new Dictionary<string, Control>();
        Control _activeControl;

        public Region(Control container)
        {
            _container = container;
        }

        public void RegisterDock(UserControl control, string key)
        {
            _container.Sync(() =>
                {
                    control.Visible = false;
                    control.Dock = DockStyle.Fill;
                    _container.Controls.Add(control);

                    _controls.Add(key, control);
                    //_activeControl = control;
                });
        }

        public void SwitchTo(string key)
        {
            _container.Sync(() =>
                {
                    var requestedControl = _controls[key];
                    if (requestedControl == _activeControl)
                        return;

                    if (_activeControl != null)
                    {
                        _activeControl.Visible = false;
                    }

                    requestedControl.BringToFront();
                    requestedControl.Visible = true;
                    
                    _activeControl = requestedControl;
                    
                });
        }
    }
    

    public sealed class AppInit : Message { }

    public sealed class ProfileLoaded : Message
    {
        public readonly TrustedSystemId SystemId;

        public ProfileLoaded(TrustedSystemId systemId)
        {
            SystemId = systemId;
        }
    }

    public sealed class ClientModelLoaded : Message
    {
        
    }

    public sealed class FormLoading : Message { }

    public sealed class FormLoaded : Message{}
}
