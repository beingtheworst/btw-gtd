using System.Collections.Generic;
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
            foreach (var action in view.Actions)
            {
                if (action.Archived)
                    continue;
                if (action.Completed)
                    continue;


                
                yield return action;

                if (view.Type == ProjectType.Sequential)
                {
                    // in sequential projects, only one action is active
                    yield break;
                }
            }
        }


        public string Title { get { return "Available"; } }
        public string Description { get { return "actions not blocked, future, or on hold"; } }
        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} actions available", actionCount);
        }
    }
}