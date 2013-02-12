using System;
using System.Linq;
using System.Text;
using Gtd.Shell.Commands;
using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    public sealed class ConsoleSession
    {
        public readonly ConsoleView View;
        public TrustedSystemId SystemId;

        public IFilterCriteria CurrentFilter { get; private set; }

        public int NumberOfCHarsForGuid = 4;

        public ConsoleSession(ConsoleView view)
        {
            View = view;
            SystemId = new TrustedSystemId(1);
            CurrentFilter = new RemainingFilter();
        }

        public void UpdateFilter(IFilterCriteria filter)
        {
            CurrentFilter = filter;
        }

        public TrustedSystem GetCurrentSystem()
        {
            if (!View.Systems.ContainsKey(SystemId))
                throw new KnownConsoleInputError("Trusted system not found. Please create it first by capturing a thought");
            return View.Systems[SystemId];
        }

        public ActionView MatchAction(string match)
        {
            var system = GetCurrentSystem();
            var matches = system
                .ActionDict
                      .Where(p => Matches(p.Key.Id, match))
                      .ToArray();
            if (matches.Length == 0)
            {
                var message = string.Format("No actions match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            if (matches.Length > 1)
            {
                var message = string.Format("Multiple actions match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            return matches[0].Value;
        }


        public ProjectView MatchProject(string match)
        {
            var system = GetCurrentSystem();
            var matches = system
                .ProjectList
                      .Where(p => Matches(p.ProjectId.Id, match))
                      .ToArray();
            if (matches.Length == 0)
            {
                var message = string.Format("No projects match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            if (matches.Length > 1)
            {
                var message = string.Format("Multiple projects match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            return matches[0];
        }
        public ThoughtView MatchThought(string match)
        {
            var system = GetCurrentSystem();
            var matches = system.Thoughts
                      .Where(p => Matches(p.Id, match))
                      .ToArray();
            if (matches.Length == 0)
            {
                var message = string.Format("No thoughts match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            if (matches.Length > 1)
            {
                var message = string.Format("Multiple thoughts match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            return matches[0];
        }

        string MakePartialKey(Guid id)
        {
            return id.ToString().ToLowerInvariant().Replace("-", "").Substring(0, NumberOfCHarsForGuid);
        }

        public IItemView MatchItem(string match)
        {
            var system = GetCurrentSystem();
            var matches = system.GlobalDict.Where(p => Matches(p.Key, match)).ToArray();
            if (matches.Length == 0)
            {
                var message = string.Format("No items match criteria '{0}'", match);
                throw new KnownConsoleInputError(message);
            }
            if (matches.Length > 1)
            {
                var message = new StringBuilder();
                message.AppendFormat("Multiple items match criteria '{0}':", match);

                foreach (var pair in matches.Take(10))
                {
                    message.AppendLine();
                    message.AppendFormat("  {0} {1}", MakePartialKey(pair.Key), pair.Value.GetTitle());
                }
                if (matches.Length > 10)
                {
                    message.AppendFormat("{0} more matches not shown", matches.Length - 10);
                }

                throw new KnownConsoleInputError(message.ToString());
            }
            return matches[0].Value;
        }

        static bool Matches(Guid id, string match)
        {
            return id.ToString()
                .ToLowerInvariant()
                .Replace("-", "")
                .StartsWith(match, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}