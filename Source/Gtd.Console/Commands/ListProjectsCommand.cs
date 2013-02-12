using System;
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
            if (args.Length != 0)
            {
                throw new KnownConsoleInputError("This command expects no arguments");
                
            }
            ListProjects(env);
        }


        static void ListProjects(ConsoleEnvironment env)
        {
            var system = env.Session.GetCurrentSystem();
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