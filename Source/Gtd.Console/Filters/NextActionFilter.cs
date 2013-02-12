using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    public sealed class NextActionFilter : IFilterCriteria
    {
        public bool IncludeAction(ProjectView project, ActionView action)
        {

            // TODO: implement
            return false;
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