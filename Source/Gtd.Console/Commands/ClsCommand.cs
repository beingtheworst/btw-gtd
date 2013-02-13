using System;

namespace Gtd.Shell.Commands
{
    class ClsCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] {"cls", "clear"}; } }
        public string Usage { get { return @"cls
    Clear console screen"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            // TODO: MSDN says this but should I do that here or let Program.cs handle it?
            // http://msdn.microsoft.com/en-us/library/system.console.clear.aspx
            // Calling the Clear method when a console application's output is redirected to a file throws a IOException.
            // To prevent this, always wrap a call to the Clear method in a try…catch block

            Console.Clear();
        }
    }
}
