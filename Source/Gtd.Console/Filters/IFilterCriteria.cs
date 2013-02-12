using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    public interface IFilterCriteria
    {
        bool IncludeAction(ProjectView project,  ActionView action);
        string Title { get; }
        string Description { get; }
        string FormatActionCount(int actionCount);
    }
}