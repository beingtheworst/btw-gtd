using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    public sealed class NextActionFilter : IFilterCriteria
    {
        public bool IncludeAction(ProjectView project, ActionView action)
        {

            // TODO: implement
            return false;
        }

        public string Title { get { return "Next"; } }
        public string Description { get { return "first available action per project"; } }
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