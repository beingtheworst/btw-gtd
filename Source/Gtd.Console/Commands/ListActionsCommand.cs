using System.Linq;

namespace Gtd.Shell.Commands
{
    class ListActionsCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] { "actions" , "cd", "la", "al"}; } }
        public string Usage { get { return @"actions [<project-id>]
    List all actions in a project or all available actions"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
                env.Log.Info("");
                env.Log.Trace("  Displaying ACTIONS from projects, filtered by the current filter. See filter command for more detail");

            if (args.Length == 1)
            {
                var projectMatch = args[0];
                ListActionsInProject(env, projectMatch);
                return;
            }

            if (args.Length == 0)
            {
                foreach (var project in env.Session.GetCurrentSystem().ProjectList)
                {
                    var filtered = env.Session.CurrentFilter.FilterActions(project).ToArray();

                    if (filtered.Length == 0) continue;

                    env.Log.Info("");
                    env.Log.Trace("PROJECT: {0} [ {1} ]  {2}", project.Outcome, project.Type, env.Session.CurrentFilter.FormatActionCount(filtered.Length));
                    env.Log.Info("");

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
                    env.Log.Info("");

                }
                return;
            }

            throw new KnownConsoleInputError("Currently this command expects project ID or nothing");

            
        }

        static void ListActionsInProject(ConsoleEnvironment env, string projectMatch)
        {
            var project = env.Session.MatchProject(projectMatch);

            var filtered = env.Session.CurrentFilter.FilterActions(project).ToArray();

            env.Log.Info("");
            env.Log.Info("PROJECT: {0} [ {1} ]", project.Outcome, project.Type);
            env.Log.Info("");

            env.Log.Debug("  {0}", env.Session.CurrentFilter.FormatActionCount(filtered.Length));
            env.Log.Info("");

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
            env.Log.Info("");
        }

    }
}