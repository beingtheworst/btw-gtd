using Gtd.Shell.LesserEvils;

namespace Gtd.Shell.Commands
{
    class ViewInboxCommand : IConsoleCommand
    {
        public string Usage { get { return "inbox"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (!env.InboxView.TenantInboxes.ContainsKey(env.Id))
            {
                env.Log.Error("Tenant not defined");
                return;
            }
            env.Log.Info("Inbox {0}", env.Id.Id);

            foreach (var entry in env.InboxView.TenantInboxes[env.Id])
            {
                var shortId = entry.ItemId.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                env.Log.Info(string.Format("  {0}  {1, -40}  {2}", shortId, entry.Thought, FormatEvil.TwitterOffestUtc(entry.Added)));
            }
        }
    }
}