using Gtd.Shell.Projections;

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
                        "rm",
                        "trash",
                        "tr"
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
            var record = env.Session.MatchItem(args[0]);

            var thought = record as ThoughtView;

            if (thought != null)
            {
                env.TrustedSystem.When(new ArchiveThought(env.Session.SystemId, thought.Id));
                env.Log.Info("Archiving thought");
                return;
            }
            var action = record as ActionView;
            if (action != null)
            {
                env.TrustedSystem.When(new ArchiveAction(env.Session.SystemId, action.Id));
                env.Log.Info("Archiving action");
                return;
            }



            throw new KnownConsoleInputError("Can't archive record:" + record.GetTitle());
        }
    }
}