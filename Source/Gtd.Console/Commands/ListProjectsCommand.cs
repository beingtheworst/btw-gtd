using Gtd.Shell.Projections;

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

            if (args.Length == 0)
            {
                ListProjects(env, system);
            }
            else if (args.Length == 1)
            {
                var project = system.GetProjectById(args[0]);
                env.Log.Info("Project: {0} ({1} actions)", project.Outcome, project.Actions.Count);

                foreach (var action in project.Actions)
                {
                    var guid = action.Id.Id;
                    var shortId = guid.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                    env.Log.Info(string.Format("  {0}  {1,-60}", shortId, action.Outcome));
                }
            }
        }

        static void ListProjects(ConsoleEnvironment env, TrustedSystem system)
        {
            var projects = system.ProjectList;
            env.Log.Info("Projects ({0} records)", projects.Count);

            foreach (var entry in projects)
            {
                var guid = entry.ProjectId.Id;

                var shortId = guid.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                env.Log.Info(string.Format("  {0} ({2}) {1, -60}", shortId, entry.Outcome, entry.Actions.Count));
            }
        }
    }
}