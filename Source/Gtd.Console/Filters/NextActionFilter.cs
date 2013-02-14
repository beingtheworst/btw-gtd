using System;
using System.Collections.Generic;
using Gtd.Shell.Projections;
using System.Linq;

namespace Gtd.Shell.Filters
{
    public sealed class NextActionFilter : IFilterCriteria
    {
        readonly AvailableFilter _filter = new AvailableFilter();
        public IEnumerable<ActionView> FilterActions(ProjectView view)
        {
            var value = _filter.FilterActions(view).FirstOrDefault();
            if (value != null)
                yield return value;
        }

        public string Title { get { return "Next"; } }
        public string Description { get { return "next thing to do in each eligible project"; } }
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