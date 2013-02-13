using System;
using System.Linq;
using Gtd.Shell.Projections;

namespace Gtd.Shell.Commands
{
    class ListProjectsCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] {"list", "ls", "lp", "pl"};} }
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
            env.Log.Trace("Projects ({0} records)", projects.Count);

            foreach (var entry in projects)
            {
                var guid = entry.ProjectId.Id;

                var shortId = env.Session.MakePartialKey(guid);
                var filtered = env.Session.CurrentFilter.FilterActions(entry);
                var format = env.Session.CurrentFilter.FormatActionCount(filtered.Count());
                env.Log.Info(string.Format("  {0} {1, -60} ({2}; {3})", shortId, entry.Outcome, format, entry.Type));
            }
        }
    }
}