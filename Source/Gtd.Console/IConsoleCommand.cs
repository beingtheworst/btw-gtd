using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gtd.Shell
{
    /// <summary>
    /// Command that can be executed within the interactive shell. Simply derive
    /// a public class from this interface (and in this assembly), and it will be 
    /// auto-loaded per startup
    /// </summary>
    public interface IConsoleCommand
    {
        string[] Key { get; }
        string Usage { get; }
        void Execute(ConsoleEnvironment env, string[] args);
    }

    public static class ConsoleCommands
    {
        public static IDictionary<string, IConsoleCommand> Actions = new Dictionary<string, IConsoleCommand>(StringComparer.InvariantCultureIgnoreCase);

        static ConsoleCommands()
        {
            var consoleCommandTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IConsoleCommand).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToArray();


            foreach (var type in consoleCommandTypes)
            {
                var instance = (IConsoleCommand) Activator.CreateInstance(type);

                foreach (var s in instance.Key)
                {
                    Actions.Add(s, instance);
                }
            }
        }
    }
}