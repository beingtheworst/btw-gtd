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
            var thought = env.Session.MatchThought(args[0]);
            env.TrustedSystem.When(new ArchiveThought(env.Session.SystemId, thought.Id));
            env.Log.Info("Archived");
        }

        
    }
}