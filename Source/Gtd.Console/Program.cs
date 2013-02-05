using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Gtd.CoreDomain;
using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    class Program
    {
        static ILogger Log = LogManager.GetLoggerFor<Program>();
        static void Main(string[] args)
        {
            // setup and wire our console environment
            Log.Info("Starting Being The Worst interactive shell :)");
            
            var env = ConsoleEnvironment.Build();
            Log.Info("Type 'help' to get more info");


            

            while (true)
            {
                Thread.Sleep(300);
                Console.Write("> ");
                var line = System.Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                var split = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                IConsoleCommand value;
                if (!env.Commands.TryGetValue(split[0], out value))
                {
                    Log.Error("Unknown command '{0}'. Type 'help' for help", line);
                    continue;
                }
                try
                {
                    value.Execute(env, split.Skip(1).ToArray());
                }
                catch (DomainError ex)
                {
                    Log.Error("{0}: {1}", ex.Name, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Log.Error("Invalid usage of '{0}': {1}", split[0], ex.Message);
                    Log.Debug(value.Usage);
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex, "Failure while processing command '{0}'", split[0]);
                }
            }

 
        }
    }

    public sealed class ConsoleEnvironment
    {
        public IEventStore Store { get; private set; }
        public ITenantApplicationService Tenant { get; private set; }
        public IDictionary<string, IConsoleCommand> Commands { get; private set; }
        public readonly ILogger Log = LogManager.GetLoggerFor<ConsoleEnvironment>();
        public TenantId Id { get; private set; }
        public static ConsoleEnvironment Build()
        {
            var handler = new SynchronousEventHandler();

            var inbox = new InboxProjection();
            handler.RegisterHandler(inbox);

            //var store = new InMemoryStore(handler);
            

            var store = new FileAppendOnlyStore(new DirectoryInfo(Directory.GetCurrentDirectory()));
            store.Initialize();
            var messageStore = new MessageStore(store);
            messageStore.LoadDataContractsFromAssemblyOf(typeof(ActionDefined));
            var currentVersion = store.GetCurrentVersion();
            var log = LogManager.GetLoggerFor<ConsoleEnvironment>();
            log.Debug("Event Store ver {0}", currentVersion);

            if (currentVersion > 0)
            {
                log.Debug("Running in-memory replay");
                foreach (var record in messageStore.EnumerateAllItems(0, int.MaxValue))
                {
                    foreach (var item in record.Items.OfType<Event>())
                    {
                        handler.Handle(item);
                    }
                }
                log.Debug("Replay complete");
            }
            
            
            var events = new EventStore(messageStore);



            var tenant = new TenantAppService(events, new RealTimeProvider());
            return new ConsoleEnvironment
                {
                    Store = events,
                    Tenant = tenant,
                    Commands = ConsoleCommands.Actions,
                    Id = new TenantId(1)
                };
        }

    }
}
