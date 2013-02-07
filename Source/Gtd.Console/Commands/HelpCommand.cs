using System.Linq;

namespace Gtd.Shell.Commands
{
    class HelpCommand : IConsoleCommand
    {
        public string Key { get { return "help"; } }
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
            env.Log.Info("Available commands");
            foreach (var actionHandler in env.Commands.OrderBy(h => h.Key))
            {
                env.Log.Info("  {0}", actionHandler.Key.ToUpperInvariant());
                if (!string.IsNullOrWhiteSpace(actionHandler.Value.Usage))
                {
                    env.Log.Info("    {0}", actionHandler.Value.Usage);
                }
            }

        }
    }
}