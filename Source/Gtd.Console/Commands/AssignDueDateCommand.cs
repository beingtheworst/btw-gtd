using System;
using System.Linq;

namespace Gtd.Shell.Commands
{
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
                throw new KnownConsoleInputError("Must provide action id and due date (or '-' to remove)");
            }

            var match = env.Session.MatchAction(args[0]);
            DateTime span;
            string value = string.Join(" ", args.Skip(1));
            if (value.Trim() == "-")
            {
                env.Log.Trace("Removing due date");
                env.TrustedSystem.When(new ProvideDueDateForAction(env.Session.SystemId, match.Id, DateTime.MinValue));
                return;
            }

            if (!FriendlyDateParser.TryParseDate(value, out span))
            {
                throw new KnownConsoleInputError("Failed to parse due date '{0}'", value);
            }
            env.Log.Trace("Setting due date to {0}", span);
            env.TrustedSystem.When(new ProvideDueDateForAction(env.Session.SystemId, match.Id, span));
        }
    }
}