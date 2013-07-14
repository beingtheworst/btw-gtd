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
            // this Client App uses a standard "Windows Forms" application as its host
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region This WinForms host uses its own in-memory message bus to manage the UI...
            // It uses this in-memory bus to wire user-interface "UI" elements
            // to the logic that will be triggered when the user interacts with those elements.
            // This allows us to send messages to the uiBus so that interested UI classes can 
            // subscribe to the messages they care about, and react to them
            // when the bus publishes the messages to tell the UI controls what happened.
            #endregion
            var uiBus = new InMemoryBus("UI");

            // .dat file to write Domain Events to this session
            var fileToStoreEventsIn = 
                new FileAppendOnlyStore(new DirectoryInfo(Directory.GetCurrentDirectory()));
            fileToStoreEventsIn.Initialize();

            // provide serialization stuff for our file storage
            var messageStore = new MessageStore(fileToStoreEventsIn);
            messageStore.LoadDataContractsFromAssemblyOf(typeof(ActionDefined));

            // this WinForm App's own local file-based event storage
            var appEventStore = new AppEventStore(messageStore);

            #region Does App care about this msg? This class controls the App's "main message loop"...
            // It reads all msgs from in-memory queue (mainQueue below) and determines which messages 
            // the App will/will not handle at a given time (based on specific app state).
            // For each queued message that it determines should be handled by
            // uiBus subscribers, it passes the messages through to our in-memory uiBus,
            // so bus subscribers can react to the current message when the bus publishes it.
            #endregion
            var appController = new AppController(uiBus, appEventStore);

            #region This is an in-memory structure through which all events we defined will go...
            // All domain & system messages we defined are captured 
            // and accumulated in this queue until some code picks up
            // each message and processes it.
            // (ex: AppController, declared above, will do that processing in this case).
            #endregion
            var mainQueue = new QueuedHandler(appController, "Main Queue");

            appController.SetMainQueue(mainQueue);
            appEventStore.SetDispatcher(mainQueue);

            var provider = new ClientPerspective();

            ClientPerspectiveController.WireTo(appEventStore, provider, uiBus, mainQueue);
            
            // create services and bind them to the bus

            // we wire all controls together in a native way.
            // then we add adapters on top of that

            var form = new MainForm();

            var filterService = new FilterService();

            MainFormController.Wire(form, mainQueue, uiBus, filterService);
            InboxController.Wire(form.MainRegion, mainQueue, uiBus, provider);
            NavigationController.Wire(form.NavigationRegion, mainQueue, uiBus, provider);
            ProjectController.Wire(form.MainRegion, mainQueue, uiBus, provider);
            LogController.Wire(form, uiBus);

            CaptureThoughtController.Wire(new CaptureThoughtForm(form), uiBus, mainQueue);
            DefineProjectController.Wire(new DefineProjectForm(form), uiBus, mainQueue);

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
