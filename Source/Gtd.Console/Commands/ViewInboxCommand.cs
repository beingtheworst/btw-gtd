using Gtd.Shell.LesserEvils;

namespace Gtd.Shell.Commands
{
    class ViewInboxCommand : IConsoleCommand
    {
        public string Usage { get { return "inbox"; } }
        public string[] Key { get { return new[] {"inbox", "in"}; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            var entries = env.Session.GetCurrentSystem();
            var thoughts = entries.Thoughts;
            env.Log.Info("Inbox ({0} records)", thoughts.Count);

            
            foreach (var entry in thoughts)
            {
                var shortId = entry.Id.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                env.Log.Info(string.Format("  {0}  {1, -60}  {2,10}", shortId, entry.Subject, FormatEvil.TwitterOffestUtc(entry.Added)));
            }
        }
    }
}