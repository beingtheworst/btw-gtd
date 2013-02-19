using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Gtd.CoreDomain;
using Gtd.Redis;
using Gtd.Shell.Commands;
using Gtd.Shell.Filters;
using Gtd.Shell.Projections;
using ServiceStack.Redis;

namespace Gtd.Shell
{
    class Program
    {
        static ILogger Log = LogManager.GetLoggerFor<Program>();
        static void Main(string[] args)
        {
            // setup and wire our console environment
            Log.Info("Starting Being The Worst interactive GTD shell :)");

            var setup = new Setup();
            setup.UseRedis = true;

            Log.Info(setup.ToString());

            Log.Info("");
            var env = ConsoleEnvironment.Build(setup);
            Log.Info("");
            Log.Info("Type 'help' to get more info");
            Log.Info("");

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
    }

    public class Setup
    {
        public bool UseRedis;

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendFormat("UseRedis={0}", UseRedis).AppendLine();

            return builder.ToString();
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

        public DateTime CurrentDate { get { return DateTime.Now; } }

        public static ConsoleEnvironment Build(Setup setup)
        {
            var handler = new SynchronousEventHandler();

            var inbox = new ConsoleProjection();
            handler.RegisterHandler(inbox);
            IAppendOnlyStore store;
            if (setup.UseRedis)
            {
                store = new RedisAppendOnlyStore(new RedisClient());
                try
                {
                    store.GetCurrentVersion();
                }
                catch (RedisException ex)
                {
                    throw new ApplicationException("It looks like redis is not running. Please start Library\\Redis.Win\\redis.server.exe :)");
                }
            }
            else
            {
                var file = new FileAppendOnlyStore(new DirectoryInfo(Directory.GetCurrentDirectory()));
                file.Initialize();
                store = file;
            }

            
            var messageStore = new MessageStore(store);
            messageStore.LoadDataContractsFromAssemblyOf(typeof(ActionDefined));
            var currentVersion = store.GetCurrentVersion();

            // setup Window size values for Console Window that is 60% of Max Possible Size
            int winWidth = (Console.LargestWindowWidth * 6 / 10);
            int winHeight = (Console.LargestWindowHeight * 6 / 10);

            // hack - for now, hard code "bigger" buffer than Window sizes above
            // keep horizontal buffer equal to width - to avoid horizontal scrolling
            int winBuffWidth = winWidth;
            int winBuffHeight = winHeight + 300;
            Console.SetBufferSize(winBuffWidth, winBuffHeight);

            // Buffer is bigger than Window so set the Window Size
            Console.SetWindowSize(winWidth, winHeight);

            // note that various tricks to center Console Window on launch 
            // and to change to Font size were ugly (PInvoke, etc.) so left them out for now

            Console.Title = "GTD Interactive Shell - Using Trusted System Id#: 1";

            var log = LogManager.GetLoggerFor<ConsoleEnvironment>();

            if (currentVersion < 1)
            {
                log.Debug("Event Stream ver {0}.  Trusted System has no Event history yet, capture a thought (CT).", currentVersion);
            }

            if (currentVersion > 0)
            {
                log.Debug("Event Stream ver {0} is what your Trusted System is becoming aware of...", currentVersion);
                log.Debug("Running in-memory replay of all Events that have ever happened to Trusted System: 1");
                foreach (var record in messageStore.EnumerateAllItems(0, int.MaxValue))
                {
                    foreach (var item in record.Items.OfType<Event>())
                    {
                        handler.Handle(item);
                    }
                }
                log.Debug("Event Replay complete.");
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
                    Filters = FilterCriteria.LoadAllFilters().ToList()
                };
            return build;
        }
    }
}
