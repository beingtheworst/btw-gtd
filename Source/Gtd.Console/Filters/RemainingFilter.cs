using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    /// <summary>
    /// includes blocked, future, and on-hold actions
    /// </summary>
    public sealed class RemainingFilter : IFilterCriteria
    {
        public bool IncludeAction(ProjectView proect, ActionView view)
        {
            if (view.Archived)
                return false;
            if (view.Completed)
                return false;

            return true;
        }

        public string Title { get { return "Remaining"; } }
        public string Description { get { return "anything that’s not completed"; } }

        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} remaining", actionCount);
        }
    }
}