using System;
using System.Collections.Generic;
using Gtd.Shell.Projections;
using System.Linq;

namespace Gtd.Shell.Filters
{
    public sealed class NextActionFilter : IFilterCriteria
    {

        public IEnumerable<ActionView> FilterActions(ProjectView view)
        {
            if (view.Type == ProjectType.List)
            {
                // in list, every available action is next
                foreach (var actionView in GetAllAvailableActions(view))
                {
                    yield return actionView;
                }
            }
            else if (view.Type == ProjectType.Parallel)
            {
                // in parallel, first available action is next
                var filtered = GetAllAvailableActions(view).FirstOrDefault();

                if (filtered != null)
                {
                    yield return filtered;
                }
            }
            else
            {
                // in sequential, first available action is next (unless it's blocked)
                var filtered = view.Actions
                                   .Where(v => !v.Completed)
                                   .Where(v => !v.Archived)
                                   .FirstOrDefault();

                if (filtered != null && filtered.StartDate <= DateTime.UtcNow)
                    yield return filtered;

            }

        }

        static IEnumerable<ActionView> GetAllAvailableActions(ProjectView view)
        {
            return view.Actions
                       .Where(v => !v.Completed)
                       .Where(v => !v.Archived)
                       .Where(v => v.StartDate <= DateTime.Now);
        }

        public string Title
        {
            get { return "Next"; }
        }

        public string Description
        {
            get { return "next thing to do in each eligible project"; }
        }

        public string FormatActionCount(int actionCount)
        {
            switch (actionCount)
            {
                case 0:
                    return "no next actions";
                case 1:
                    return "1 next action";
                default:
                    return string.Format("{0} next actions", actionCount);
            }
        }
    }
}