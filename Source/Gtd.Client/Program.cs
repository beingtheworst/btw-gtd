using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Btw.Portable;
using Gtd.Client.Views.Actions;
using Gtd.CoreDomain;

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

            var view = new SystemProjection(events);
            view.SubscribeTo(bus);

            // create services and bind them to the bus

            // we wire all controls together in a native way.
            // then we add adapters on top of that

            var form = new MainForm();


            var filterService = new FilterService();

            MainFormController.Wire(form, mainQueue, bus,filterService);
            InboxAdapter.Wire(form.MainRegion, mainQueue, bus, view);
            NavigationAdapter.Wire(form.NavigationRegion, mainQueue, bus, view);
            ProjectAdapter.Wire(form.MainRegion, mainQueue, bus, view);
            LogController.Wire(form, bus);

            

            mainQueue.Enqueue(new AppInit());
            mainQueue.Start();
            
            Application.Run(form);
        }
    }


    public sealed class AppEventStore : IEventStore
    {
        readonly MessageStore _store;
         IHandle<Message> _handler;

        public AppEventStore(MessageStore store)
        {
            _store = store;
        }

        public void SetDispatcher(IHandle<Message> dispatcher)
        {
            _handler = dispatcher;
        }

        public void AppendEventsToStream(string name, long streamVersion, ICollection<Event> events)
        {
            if (events.Count == 0) return;
            // functional events don't have an identity

            try
            {
                _store.AppendToStore(name, streamVersion, events.Cast<object>().ToArray());
            }
            catch (AppendOnlyStoreConcurrencyException e)
            {
                // load server events
                var server = LoadEventStream(name);
                // throw a real problem
                throw OptimisticConcurrencyException.Create(server.StreamVersion, e.ExpectedStreamVersion, name, server.Events);
            }
            // sync handling. Normally we would push this to async
            foreach (var @event in events)
            {
                _handler.Handle(@event);
            }
        }
        public EventStream LoadEventStream(string name)
        {


            // TODO: make this lazy somehow?
            var stream = new EventStream();
            foreach (var record in _store.EnumerateMessages(name, 0, int.MaxValue))
            {
                stream.Events.AddRange(record.Items.Cast<Event>());
                stream.StreamVersion = record.StreamVersion;
            }
            return stream;
        }
    }


    
    public sealed class Region 
    {
        readonly Control _container;
        IDictionary<string,Control> _controls= new Dictionary<string, Control>();
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


    

    public sealed class AppInit : Message
    {
        
        
    }

    public sealed class FormLoading : Message
    {
        
    }

    public sealed class FormLoaded : Message{}
}
