using System;
using System.Linq;

namespace Gtd.Shell.Commands
{
    class ArchiveThoughtCommand : IConsoleCommand
    {
        public string[] Key
        {
            get
            {
                return new[]
                    {
                        "archive",
                        "at"
                    };
            }
        }

        public string Usage { get { return @"archive <thought-id>
    Archives thought from the inbox (hiding it from there). You need to provide first digits of thought id."; } }


        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must specify ID of the thought to archive");
                return;
            }

            var inbox = env.ConsoleView.Systems[env.Id].Thoughts;

            var matches = inbox.Where(t => Matches(t.ItemId, args[0])).ToArray();
            if (matches.Length == 0)
            {
                env.Log.Error("Nothing to archive");
                return;
            }
            if (matches.Length == 1)
            {
                env.TrustedSystem.When(new ArchiveThought(env.Id, matches[0].ItemId));
                env.Log.Info("Archived");
                return;
            }
            env.Log.Error("{0} thoughts match '{1}' criteria.", matches.Length, args[0]);
        }

        static bool Matches(Guid id, string match)
        {
            return id.ToString().ToLowerInvariant().Replace("-", "").StartsWith(match);
        }
    }
}