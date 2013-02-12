using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Gtd.CoreDomain;
using Gtd.Shell.Commands;
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

            SetupConsoleWindow();

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
                catch (KnownConsoleInputError ex)
                {
                    Log.Error(ex.Message);
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex, "Failure while processing command '{0}'", split[0]);
                }
            }

 
        }

        static void SetupConsoleWindow()
        {
            // HACK 
            // try to mess with Console Window size and layout here

            Console.Title = "GTD Interactive Shell - beingtheworst.com";

            // setup Window size values for Console Window that is 60% of Max Possible Size
            int winWidth = (Console.LargestWindowWidth * 6 / 10);
            int winHeight = (Console.LargestWindowHeight * 6 / 10);

            // hack - for now, hard code "bigger" buffer than Window sizes above
            int winBuffWidth = winWidth + 80;
            int winBuffHeight = winHeight + 300;
            Console.SetBufferSize(winBuffWidth, winBuffHeight);

            // Buffer is bigger than Window so set the Window Size
            Console.SetWindowSize(winWidth, winHeight);

            // note that various tricks to center Console Window on launch 
            // and to change to Font size were ugly so left them out for now
        }
    }

    public sealed class ConsoleEnvironment
    {
        public IEventStore Store { get; private set; }
        public ITrustedSystemApplicationService TrustedSystem { get; private set; }
        public IDictionary<string, IConsoleCommand> Commands { get; private set; }
        public readonly ILogger Log = LogManager.GetLoggerFor<ConsoleEnvironment>();
        


        public ConsoleView ConsoleView { get; private set; }
        public ConsoleSession Session { get; private set; }
        public IList<IFilterCriteria> Filters { get; private set; } 




        public static ConsoleEnvironment Build()
        {
            var handler = new SynchronousEventHandler();

            var inbox = new ConsoleProjection();
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
            
            var events = new EventStore(messageStore,handler);

            var trustedSystem = new TrustedSystemAppService(events, new RealTimeProvider());
            var build = new ConsoleEnvironment
                {
                    Store = events, 
                    TrustedSystem = trustedSystem, 
                    Commands = ConsoleCommands.Actions, 
                    Session = new ConsoleSession(inbox.ViewInstance),
                    ConsoleView = inbox.ViewInstance,
                    Filters = FilterCriteria.LoadALlFilters().ToList()
                };
            return build;
        }
    }

}
