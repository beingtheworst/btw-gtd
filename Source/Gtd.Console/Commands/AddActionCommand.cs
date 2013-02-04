using System;

namespace Gtd.Shell.Commands
{
    class AddActionCommand : IConsoleCommand
    {
        public string Usage
        {
            get { return "add [action name]"; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must specify action name");
                return;
            }
            env.Tenant.When(new DefineAction(env.Id, Guid.Empty, string.Join(" ", args)));
        }
    }
}