namespace Gtd.Shell.Commands
{
    class ViewInboxCommand : IConsoleCommand
    {
        public string Usage { get { return "inbox"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (!env.Inbox.Inbox.ContainsKey(env.Id))
            {
                env.Log.Error("Tenant not defined");
                return;
            }
            env.Log.Info("Inbox {0}", env.Id.Id);

            foreach (var entry in env.Inbox.Inbox[env.Id].Items)
            {
                env.Log.Info(entry);
            }
        }
    }
}