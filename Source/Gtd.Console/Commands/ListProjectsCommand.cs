using Gtd.Shell.Projections;
using System.Linq;

namespace Gtd.Shell.Commands
{

    class CompleteActionCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] { "complete", "ca"};} }
        public string Usage { get; private set; }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                throw new KnownConsoleInputError("Must provide ID of action to complete");
            }
            var action = env.Session.MatchAction(args[0]);
            if (action.Completed)
                throw new KnownConsoleInputError("Action is already completed.");

            env.TrustedSystem.When(new CompleteAction(env.Session.SystemId, action.Id));
            env.Log.Debug("Action '{0}' marked as completed. Good job!", action.Outcome);
        }
    }
    class ListProjectsCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] {"list", "ls"};} }
        public string Usage { get { return @"list
    Return list of all projects available"; } }

        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                ListProjects(env);
            }
            else if (args.Length == 1)
            {
                var projectMatch = args[0];
                ListActionsInProject(env, projectMatch);
            }
        }

        static void ListActionsInProject(ConsoleEnvironment env, string projectMatch)
        {
            var project = env.Session.MatchProject(projectMatch);

            var filtered = project.Actions.Where(a => env.Session.CurrentFilter.IncludeAction(project,a)).ToArray();

            env.Log.Info("Project: {0} [ {1} ]", project.Outcome, project.Type);

            env.Log.Debug("  {0}", env.Session.CurrentFilter.FormatActionCount(filtered.Length));

            foreach (var action in filtered)
            {
                var guid = action.Id.Id;
                var shortId = guid.ToString().ToLowerInvariant().Replace("-", "").Substring(0, 3);
                env.Log.Info(string.Format("  [{0}]  {1,-60} {2}", action.Completed ? "X" : " ", action.Outcome,
                    shortId));

            }
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

    public sealed class ChangeProjectTypeCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] { "type", "cp"};} }
        public string Usage { get { return @"type <projectId> seq | si | par
    Change project type"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length != 2)
            {
                throw new KnownConsoleInputError("Expected 2 arguments");
            }

            var project = env.Session.MatchProject(args[0]);
            ProjectType  changeTo;

            var lowerInvariant = args[1].ToLowerInvariant();
            if (lowerInvariant.StartsWith("p"))
            {
                changeTo = ProjectType.Parallel;
            }
            else if (lowerInvariant.StartsWith("se"))
            {
                changeTo = ProjectType.Sequential;
            }
            else if (lowerInvariant.StartsWith("si"))
            {
                changeTo = ProjectType.SingleActions;
            }
            else
            {
                var message = string.Format("Unknown project type {0}", lowerInvariant);
                throw new KnownConsoleInputError(message);
            }


            env.TrustedSystem.When(new Gtd.ChangeProjectType(env.Session.SystemId, project.ProjectId, changeTo));
            env.Log.Trace("Projec type changed");
        }
    }
}