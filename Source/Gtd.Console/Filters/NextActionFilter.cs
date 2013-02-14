using System;
using System.Collections.Generic;
using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    public sealed class NextActionFilter : IFilterCriteria
    {
        public IEnumerable<ActionView> FilterActions(ProjectView view)
        {
            foreach (var action in view.Actions)
            {
                if (action.Completed)
                    continue;
                if (action.Archived)
                    continue;
                yield return action;

                if (view.Type != ProjectType.List)
                    yield break;

                // single actions lists allow multiple next actions
            }
            
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