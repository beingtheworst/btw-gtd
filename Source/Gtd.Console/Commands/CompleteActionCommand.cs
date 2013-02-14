using System;
using System.Collections.Generic;
using System.Linq;
using Gtd.Shell.Projections;

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

    class ForecastViewCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[]{"forecast", "overdue"};} }
        public string Usage { get { return @"forecast
    Displays overdue items"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            var system = env.Session.GetCurrentSystem();


            var open = system.ActionDict.Values
                .Where(v => !v.Completed)
                .Where(v => !v.Archived)
                .Where(v => v.DueDate != DateTime.MinValue)
                .ToList();
            var pastDue = open
                .Where(v => v.DueDate.Date < env.CurrentDate).ToList();
            if (pastDue.Count > 0)
            {
                env.Log.Info("Past DUE actions");

                foreach (var action in pastDue)
                {
                    PrintAction(env, action);
                }
            }
            var today = env.CurrentDate.Date;

            for (int i = 0; i < 7; i++)
            {
                var day = today.AddDays(i);
                var dueThisDay = open.Where(v => v.DueDate >= day && v.DueDate < (day.AddDays(1))).ToArray();
                if (dueThisDay.Length > 0)
                {
                    env.Log.Info("Due {0}", day.ToLongDateString());
                    foreach (var action in dueThisDay)
                    {
                        PrintAction(env, action);
                    }
                }
            }




        }

        static void PrintAction(ConsoleEnvironment env, ActionView action)
        {
            var shortId = env.Session.MakePartialKey(action.Id.Id);
            env.Log.Info(string.Format("  [{0}] {1,-60}          {2}", action.Completed ? "V" : " ", action.Outcome,
                shortId));
        }
    }
}