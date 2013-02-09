using System;
using System.Linq;
using Gtd.Shell.Commands;
using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    public sealed class ConsoleSession
    {
        public readonly ConsoleView View;
        public TrustedSystemId SystemId;

        public ConsoleSession(ConsoleView view)
        {
            View = view;
            SystemId = new TrustedSystemId(1);
        }

        public TrustedSystem GetCurrentSystem()
        {
            if (!View.Systems.ContainsKey(SystemId))
                throw new KnownConsoleInputError("Trusted system not found. Please create it first by capturing a thought");
            return View.Systems[SystemId];
        }


        public Project MatchProject(string match)
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
        public Thought MatchThought(string match)
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

        static bool Matches(Guid id, string match)
        {
            return id.ToString()
                .ToLowerInvariant()
                .Replace("-", "")
                .StartsWith(match, StringComparison.InvariantCultureIgnoreCase);
        }
        
    }
}