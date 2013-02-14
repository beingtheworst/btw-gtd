using System;
using System.Globalization;
using System.Linq;
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

    public static class Parser
    {
        public delegate DateTime Change(float diff, DateTime source);

        static bool TryRepresent(string value, string[] suffix, Change producer, out DateTime result)
        {
            result = DateTime.MinValue;
            foreach (var s in suffix)
            {
                if (!value.EndsWith(s, StringComparison.InvariantCultureIgnoreCase))
                    continue;
                var trimmed = value.Remove(value.Length - s.Length, s.Length).TrimEnd();
                float res;

                if (!float.TryParse(trimmed, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out res))
                {
                    continue;
                }
                result =producer(res, DateTime.Now);
                return true;
            }
            return false;
        }

        public static bool TryParseDate(string value, out DateTime span)
        {
            span = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim();

            try
            {
                if (TryRepresent(value, new[] {"w", "wk", "week"}, (diff, source) => source.AddDays(7 * diff) , out span))
                    return true;
                if (TryRepresent(value, new string[] {"d", "day", "days"},(diff, source) => source.AddDays(7*diff), out span))
                    return true;
                if (TryRepresent(value, new string[] {"m", "mth", "month"}, (diff, source) => source.AddMonths((int)diff),out span)) ;

                

            }
            catch (Exception ex)
            {
                
             
            }
            return false;

        }
    }
}