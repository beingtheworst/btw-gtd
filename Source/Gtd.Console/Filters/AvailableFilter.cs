using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    /// <summary>
    /// things you can actually do, because they aren’t blocked by earlier actions in a sequential project
    /// </summary>
    public sealed class AvailableFilter : IFilterCriteria
    {
        public bool IncludeAction(ProjectView project, ActionView action)
        {
            if (action.Archived)
                return false;
            if (action.Completed)
                return false;
            return true;
        }

        public string Title { get { return "Available"; } }
        public string Description { get { return "actions not blocked, future, or on hold"; } }
        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} available", actionCount);
        }
    }
}