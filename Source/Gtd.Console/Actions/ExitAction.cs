using System;

namespace Gtd.Shell.Actions
{
    class ExitAction : IConsoleAction
    {
        public string Usage { get { return "exit"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            Environment.Exit(0);
        }
    }
}