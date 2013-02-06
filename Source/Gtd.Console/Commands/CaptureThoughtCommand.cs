using System;

namespace Gtd.Shell.Commands
{
    class CaptureThoughtCommand : IConsoleCommand
    {
        public string Usage
        {
            get { return "capture [action name]"; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must specify action name");
                return;
            }
            env.Tenant.When(new CaptureThought(env.Id, Guid.Empty, string.Join(" ", args)));
            env.Log.Info("Thought captured safely!");
        }
    }
}