using System;
using System.Linq;

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

            var requestId = new RequestId(Guid.NewGuid());

            var project = env.Session.MatchProject(args[0]);

            var outcome = string.Join(" ", args.Skip(1));
            env.TrustedSystem.When(new DefineAction(env.Session.SystemId, requestId, project.ProjectId, outcome));
            env.Log.Trace("Action defined for project '{0}'", project.Outcome);
        }
    }
}