using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Gtd.CoreDomain;

namespace Gtd.Shell
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup and wire our console environment
            var env = ConsoleEnvironment.Build();
            env.Log.Info("Starting Being The Worst interactive shell :)");
            env.Log.Info("Type 'help' to get more info");

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
                    env.Log.Error("Unknown command '{0}'. Type 'help' for help", line);
                    continue;
                }
                try
                {
                    value.Execute(env, split.Skip(1).ToArray());
                }
                catch (DomainError ex)
                {
                    env.Log.Error("{0}: {1}", ex.Name, ex.Message);
                }
                catch (ArgumentException ex)
                {
                    env.Log.Error("Invalid usage of '{0}': {1}", split[0], ex.Message);
                    env.Log.Debug(value.Usage);
                }
                catch (Exception ex)
                {
                    env.Log.ErrorException(ex, "Failure while processing command '{0}'", split[0]);
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
            //var store = new InMemoryStore(handler);
            

            var store = new FileAppendOnlyStore(new DirectoryInfo(Directory.GetCurrentDirectory()));
            store.Initialize();
            var messageStore = new MessageStore(store);
            messageStore.LoadDataContractsFromAssemblyOf(typeof(ActionDefined));
            var events = new EventStore(messageStore);

            var tenant = new TenantAppService(events, new RealTimeProvider());
            return new ConsoleEnvironment()
                {
                    Store = events,
                    Tenant = tenant,
                    Commands = ConsoleCommands.Actions,
                    Id = new TenantId(1)
                    
                };
            
        }

    }
}
