using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    public sealed class AllActionsFilter : IFilterCriteria
    {
        public bool IncludeAction(ProjectView project, ActionView action)
        {
            return true;
        }

        public string Title { get { return "All actions"; } }
        public string Description { get { return "includes completed and archived items"; } }

        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} total", actionCount);
        }
    }
}