using System.Linq;

namespace Gtd.Shell.Commands
{
    class HelpCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] {"help","?"}; } }
        public string Usage { get { return "help [<command>]"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length > 0)
            {
                IConsoleCommand value;
                if (!env.Commands.TryGetValue(args[0], out value))
                {
                    env.Log.Error("Can't find help for '{0}'", args[0]);
                    return;
                }
                env.Log.Info(value.Usage ?? "No Help available");
                return;
            }
            env.Log.Info("");
            env.Log.Info("Available commands");
            env.Log.Info("");
            foreach (var group in env.Commands.GroupBy(p => p.Value).OrderBy(c => c.GetType().Name))
            {
                var keys = string.Join(", ", group.Select(p => p.Key.ToUpperInvariant()));
                env.Log.Info("  {0}", keys);
                var usage = @group.Key.Usage;
                if (!string.IsNullOrWhiteSpace(usage))
                {
                    env.Log.Debug("    {0}", usage);
                }
            }
            env.Log.Info("");
        }
    }
}