using System;
using System.Linq;

namespace Gtd.Shell.Commands
{
    class ExitCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] {"exit", "quit","q"}; } }
        public string Usage { get { return @"exit
    Exit this console"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            Environment.Exit(0);
        }
    }

    class ActionStartDateCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] {"start", "sd"}; } }
        public string Usage { get; private set; }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                throw new KnownConsoleInputError("Must provide action id and start date");
            }

            var match = env.Session.MatchAction(args[0]);
            DateTime span;
            string value = string.Join(" ", args.Skip(1));
            if (!Parser.TryParseDate(value, out span))
            {
                throw new KnownConsoleInputError("Failed to parse start date '{0}'", value);
            }
            env.Log.Trace("Setting start date to {0}", span);
            env.TrustedSystem.When(new ProvideStartDateForAction(env.Session.SystemId, match.Id, span));
        }
    }
}