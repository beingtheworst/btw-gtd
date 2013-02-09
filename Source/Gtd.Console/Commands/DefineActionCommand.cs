using System;
using System.Linq;
using Gtd.Shell.Projections;

namespace Gtd.Shell.Commands
{
    class DefineActionCommand : IConsoleCommand
    {
        public string[] Key { get {return new string[]{"action","da"};}}
        public string Usage { get { return "action <projectId> <action outcome>"; } }

        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length < 2)
            {
                env.Log.Error("You must provide project ID and action outcome");
                return;
            }
            var project = env.Session.MatchProject(args[0]);

            var outcome = string.Join(" ", args.Skip(1));
            env.TrustedSystem.When(new DefineAction(env.Session.SystemId, Guid.Empty, project.ProjectId, outcome));
            env.Log.Trace("Action defined for project '{0}'", project.Outcome);
        }
    }

    class RenameSubjectCommand : IConsoleCommand
    {
        public string[] Key { get {return new string[] {"rename","rn"};} }
        public string Usage
        {
            get { return @"rename <id> <new name>
    Rename action, project, folder or context."; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length < 2)
            {
                throw new KnownConsoleInputError("At least 2 arguments are expected");
            }
            var item = env.Session.MatchItem(args[0]);
            var newName = string.Join(" ", args.Skip(1));

            var action = item as ActionView;
            if (action != null)
            {
                env.TrustedSystem.When(new ChangeActionOutcome(env.Session.SystemId, action.Id, newName));
                env.Log.Info("Action outcome changed!");
                return;
            }
            var project = item as ProjectView;
            if (project != null)
            {
                env.TrustedSystem.When(new ChangeProjectOutcome(env.Session.SystemId, project.ProjectId, newName));
                env.Log.Info("Project outcome changed!");
                return;
            }

            var thought = item as ThoughtView;

            if (thought != null)
            {
                env.TrustedSystem.When(new ChangeThoughtSubject(env.Session.SystemId, thought.Id, newName));
                env.Log.Info("Thought subject changed!");
                return;
            }
            throw new KnownConsoleInputError(string.Format("We don't support renaming {0} yet", item.GetType().Name));
        }
    }
}