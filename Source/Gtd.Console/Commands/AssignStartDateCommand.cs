using System;
using System.Linq;

namespace Gtd.Shell.Commands
{
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
                throw new KnownConsoleInputError("Must provide action id and start date (or '-' to remove)");
            }

            var match = env.Session.MatchAction(args[0]);
            DateTime span;
            string value = string.Join(" ", args.Skip(1));

            if (value.Trim() == "-")
            {
                env.Log.Trace("Removing start date");
                env.TrustedSystem.When(new ProvideStartDateForAction(env.Session.SystemId, match.Id, DateTime.MinValue));
                return;
            }
            
            if (!FriendlyDateParser.TryParseDate(value, out span))
            {
                throw new KnownConsoleInputError("Failed to parse start date '{0}'", value);
            }
            env.Log.Trace("Setting start date to {0}", span);
            env.TrustedSystem.When(new ProvideStartDateForAction(env.Session.SystemId, match.Id, span));
        }
    }
}