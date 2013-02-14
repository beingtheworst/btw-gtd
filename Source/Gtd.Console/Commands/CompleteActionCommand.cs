using System.Collections.Generic;

namespace Gtd.Shell.Commands
{
    class CompleteActionCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] { "complete", "ca"};} }
        public string Usage { get { return @"complete <actionId>
    mark action as completed"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                throw new KnownConsoleInputError("Must provide ID of action to complete");
            }
            var action = env.Session.MatchAction(args[0]);
            if (action.Completed)
                throw new KnownConsoleInputError("Action is already completed.");

            env.TrustedSystem.When(new CompleteAction(env.Session.SystemId, action.Id));
            env.Log.Debug("Action '{0}' marked as completed. Good job!", action.Outcome);
        }
    }
}