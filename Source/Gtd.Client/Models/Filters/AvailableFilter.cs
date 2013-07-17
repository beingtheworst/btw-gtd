#region (c) 2012-2013 Copyright BeingTheWorst.com

// This project is sample code that is discussed on the
// "Being the Worst" podcast.
// Subscribe to our podcast feed at:
// http://beingtheworst.com/feed
// and follow us on twitter @beingtheworst
// for more details.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Gtd.Client.Models;

namespace Gtd.Shell.Filters
{
    /// <summary>
    /// things you can actually do, because they aren’t blocked by earlier actions in a sequential project
    /// </summary>
    public sealed class AvailableFilter : IFilterCriteria
    {
        public IEnumerable<ImmutableAction> FilterActions(ImmutableProject model)
        {
            if (model.Type == ProjectType.Sequential)
            {
                var filtered = model.Actions
                                   .Where(v => !v.Completed)
                                   .FirstOrDefault(v => !v.Archived);

                if (filtered != null && filtered.StartDate <= DateTime.UtcNow)
                    yield return filtered;
            }
            else
            {
                var availableActions = model.Actions
                    .Where(v => !v.Completed)
                    .Where(v => !v.Archived);
                var filtered = availableActions
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