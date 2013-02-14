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

    class AssignStartDateCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] {"start", "sd"}; } }
        public string Usage
        {
            get
            {
                return @"due <actionId> <date>
Set optional Date when project or action becomes available or relevant";
            }
        }
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

    class AssignDueDateCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] { "due", "dd" }; } }
        public string Usage
        {
            get { return @"due <actionId> <date>
Set optional Date when project or action is past due"; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                throw new KnownConsoleInputError("Must provide action id and due date");
            }

            var match = env.Session.MatchAction(args[0]);
            DateTime span;
            string value = string.Join(" ", args.Skip(1));
            if (!Parser.TryParseDate(value, out span))
            {
                throw new KnownConsoleInputError("Failed to parse due date '{0}'", value);
            }
            env.Log.Trace("Setting due date to {0}", span);
            env.TrustedSystem.When(new ProvideDueDateForAction(env.Session.SystemId, match.Id, span));
        }
    }

}