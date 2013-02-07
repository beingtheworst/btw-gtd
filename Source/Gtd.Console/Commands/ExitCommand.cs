using System;

namespace Gtd.Shell.Commands
{
    class ExitCommand : IConsoleCommand
    {
        public string Key { get { return "exit"; } }
        public string Usage { get { return @"exit
    Exit this console"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            Environment.Exit(0);
        }
    }
}