using System;
using System.Linq;
using Gtd.Shell.Projections;

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
            env.Log.Trace("  Displaying ACTIONS from projects, filtered by filter {0}. See filter command for more detail", env.Session.CurrentFilter.Title);

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

                            PrintDetail(env, action);
                        }
                        else
                        {
                            env.Log.Info(string.Format("  [{0}] {1,-60}          {2}", action.Completed ? "V" : " ", action.Outcome,
                                shortId));
                            PrintDetail(env, action);
                        }
                    }
                    env.Log.Info("");

                }
                return;
            }

            throw new KnownConsoleInputError("Currently this command expects project ID or nothing");

            
        }

        static void PrintDetail(ConsoleEnvironment env, ActionView action)
        {
            var detail = "";
            if (action.StartDate != DateTime.MinValue)
            {
                detail += string.Format("  Starts: {0}.", action.StartDate.ToShortDateString());
            }
            if (action.DueDate != DateTime.MinValue)
            {
                detail += string.Format(" Due: {0}.", action.DueDate.ToShortDateString());
            }
            detail = detail.Trim();
            if (!string.IsNullOrWhiteSpace(detail))
            {
                env.Log.Trace("    " + detail);
            }
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