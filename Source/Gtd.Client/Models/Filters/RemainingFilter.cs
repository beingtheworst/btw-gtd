using System.Collections.Generic;
using Gtd.Client.Models;

namespace Gtd.Shell.Filters
{
    /// <summary>
    /// includes blocked, future, and on-hold actions
    /// </summary>
    public sealed class RemainingFilter : IFilterCriteria
    {
        public IEnumerable<ActionModel> FilterActions(ProjectModel model)
        {
            foreach (var action in model.Actions)
            {
                if (action.Archived)
                    continue;
                if (action.Completed)
                    continue;
                yield return action;
            }
        }
        public string Title { get { return "Remaining"; } }
        public string Description { get { return "anything that’s not completed"; } }

        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} remaining", actionCount);
        }
    }
}