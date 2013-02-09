namespace Gtd.Shell.Commands
{
    class ListProjectsCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] {"list", "ls"};} }
        public string Usage { get { return @"list
    Return list of all projects available"; } }

        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (!env.ConsoleView.Systems.ContainsKey(env.Id))
            {
                env.Log.Error("Trusted System not defined");
                return;
            }

            var system = env.ConsoleView.Systems[env.Id];
            var projects = system.ProjectList;
            env.Log.Info("Projects ({0} records)", projects.Count);

            foreach (var entry in projects)
            {
                var shortId = entry.ProjectId.Id.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                env.Log.Info(string.Format("  {0} ({2}) {1, -60}", shortId, entry.Outcome, entry.Actions.Count));
            }
        }
    }
}