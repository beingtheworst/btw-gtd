using Gtd.Shell.LesserEvils;

namespace Gtd.Shell.Commands
{
    class ViewInboxCommand : IConsoleCommand
    {
        public string Usage { get { return "inbox"; } }
        public string[] Key { get { return new[] {"ibox", "in"}; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (!env.ConsoleView.Systems.ContainsKey(env.Id))
            {
                env.Log.Error("Trusted System not defined");
                return;
            }
            var entries = env.ConsoleView.Systems[env.Id];
            var thoughts = entries.Thoughts;
            env.Log.Info("Inbox ({0} records)", thoughts.Count);

            
            foreach (var entry in thoughts)
            {
                var shortId = entry.ItemId.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                env.Log.Info(string.Format("  {0}  {1, -60}  {2,10}", shortId, entry.Subject, FormatEvil.TwitterOffestUtc(entry.Added)));
            }
        }
    }
}