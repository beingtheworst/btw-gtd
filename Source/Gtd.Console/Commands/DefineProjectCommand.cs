using System;

namespace Gtd.Shell.Commands
{
    class DefineProjectCommand : IConsoleCommand
    {
        public string Usage { get { return @"proj [project outcome]
    Define new project with a given outcome"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must provide project outcome");
                return;
            }

            var outcome = string.Join(" ", args);
            env.TrustedSystem.When(new DefineProject(env.Id, Guid.Empty, outcome));
            env.Log.Info("Project defined!");
        }
    }
}