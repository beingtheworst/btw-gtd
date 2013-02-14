#region (c) 2011-2013 BeingTheWorst.com

// This project is a sample code for http://beingtheworst.com

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    /// <summary>
    /// things you can actually do, because they aren’t blocked by earlier actions in a sequential project
    /// </summary>
    public sealed class AvailableFilter : IFilterCriteria
    {
        public IEnumerable<ActionView> FilterActions(ProjectView view)
        {
            if (view.Type == ProjectType.Sequential)
            {
                var filtered = view.Actions
                                   .Where(v => !v.Completed)
                                   .FirstOrDefault(v => !v.Archived);

                if (filtered != null && filtered.StartDate <= DateTime.UtcNow)
                    yield return filtered;
            }
            else
            {
                var filtered = view.Actions
                                   .Where(v => !v.Completed)
                                   .Where(v => !v.Archived)
                                   .Where(v => v.StartDate <= DateTime.Now);

                foreach (var action in filtered)
                {
                    yield return action;
                }
            }
        }


        public string Title
        {
            get { return "Available"; }
        }

        public string Description
        {
            get { return "actions not blocked, future, or on hold"; }
        }

        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} actions available", actionCount);
        }
    }
}