using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Gtd.Shell
{
    public interface IConsoleCommand
    {
        string Usage { get; }
        void Execute(ConsoleEnvironment env, string[] args);
    }

    public static class ConsoleCommands
    {
        public static IDictionary<string, IConsoleCommand> Actions = new Dictionary<string, IConsoleCommand>();

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
                var key = instance.Usage.Split(new[] {' '}, 2).First();
                Actions.Add(key, instance);
            }
        }
    }
}