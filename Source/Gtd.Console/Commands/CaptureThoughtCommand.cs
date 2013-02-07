using System;

namespace Gtd.Shell.Commands
{
    class CaptureThoughtCommand : IConsoleCommand
    {
        public string Usage
        {
            get { return "capture [thought description]"; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must describe the thought to capture.");
                return;
            }
            env.Tenant.When(new CaptureThought(env.Id, Guid.Empty, string.Join(" ", args)));
            env.Log.Info("Thought captured safely!");
        }
    }
}