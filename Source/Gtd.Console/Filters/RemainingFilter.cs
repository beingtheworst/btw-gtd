using Gtd.Shell.Projections;

namespace Gtd.Shell
{
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
        public string Description { get { return "includes blocked, future, and on-hold actions"; } }

        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} remaining", actionCount);
        }
    }
}