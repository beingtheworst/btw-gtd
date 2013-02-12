using System.Linq;

namespace Gtd.Shell.Commands
{
    class ListActionsCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] { "actions" , "cd"}; } }
        public string Usage { get { return @"actions [<project-id>]
List all actions in a project"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length != 1)
                throw new KnownConsoleInputError("Currently this command expects project ID or nothing");

            var projectMatch = args[0];
            ListActionsInProject(env, projectMatch);
        }

        static void ListActionsInProject(ConsoleEnvironment env, string projectMatch)
        {
            var project = env.Session.MatchProject(projectMatch);

            var filtered = env.Session.CurrentFilter.FilterActions(project).ToArray();

            env.Log.Info("Project: {0} [ {1} ]", project.Outcome, project.Type);

            env.Log.Debug("  {0}", env.Session.CurrentFilter.FormatActionCount(filtered.Length));

            foreach (var action in filtered)
            {
                var shortId = env.Session.MakePartialKey(action.Id.Id);

                if (action.Archived)
                {
                    env.Log.Trace(string.Format("  [{0}] {1,-60} Archived {2} ", action.Completed ? "V" : " ", action.Outcome,
                        shortId));
                }
                else
                {
                    env.Log.Info(string.Format("  [{0}] {1,-60}          {2}", action.Completed ? "V" : " ", action.Outcome,
                        shortId));
                }
            }
        }

    }
}