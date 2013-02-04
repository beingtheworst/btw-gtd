using System;

namespace Gtd.Shell.Commands
{
    class ExitCommand : IConsoleCommand
    {
        public string Usage { get { return "exit"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            Environment.Exit(0);
        }
    }
}