using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Btw.Portable;
using Gtd.CoreDomain;
using Gtd.Shell;
using Gtd.Shell.Projections;

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

            var form = new MainForm(mainQueue);
            var main = new MainFormController(form, mainQueue);
            main.SubscribeTo(bus);


            var inboxView = new InboxViewAdapter(form,mainQueue,view);
            inboxView.SubscribeTo(bus);

            var tree = new TreeViewController(form._tree, mainQueue,view);
            tree.SubscribeTo(bus);

            
            mainQueue.Enqueue(new AppInit());
            mainQueue.Start();
            
            Application.Run(form);



            //var handler = new SynchronousEventHandler();


            //var form = new Form1();
            //handler.RegisterHandler(form);
            //var file = new FileAppendOnlyStore(new DirectoryInfo(Directory.GetCurrentDirectory()));
            //file.Initialize();


            //var messageStore = new MessageStore(file);
            //messageStore.LoadDataContractsFromAssemblyOf(typeof(ActionDefined));

            //foreach (var record in messageStore.EnumerateAllItems(0, int.MaxValue))
            //{
            //    foreach (var item in record.Items.OfType<Event>())
            //    {
            //        handler.Handle(item);
            //    }
            //}

            //var events = new EventStore(messageStore, handler);

            //var trustedSystem = new TrustedSystemAppService(events, new RealTimeProvider());


            //form.App = trustedSystem;
            //handler.RegisterHandler(form);
            //Application.Run(form);
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


    public interface IMainDock
    {
        void RegisterDock(UserControl control, string key);
        void SwitchTo(string key);
    }


    

    public sealed class AppInit : Message
    {
        
        
    }

    public sealed class FormLoading : Message
    {
        
    }

    public sealed class FormLoaded : Message{}
}
