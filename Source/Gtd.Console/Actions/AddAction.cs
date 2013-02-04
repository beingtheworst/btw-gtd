using System;

namespace Gtd.Shell.Actions
{
    class AddAction : IConsoleAction
    {
        public string Usage
        {
            get { return "add [action name]"; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            env.Tenant.When(new DefineAction(env.Id, Guid.Empty, string.Join(" ", args)));
        }
    }
}