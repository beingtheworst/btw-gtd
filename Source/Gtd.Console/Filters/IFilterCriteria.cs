using System.Collections.Generic;
using Gtd.Shell.Projections;

namespace Gtd.Shell.Filters
{
    public interface IFilterCriteria
    {
        
        IEnumerable<ActionView> FilterActions(ProjectView view); 

        string Title { get; }
        string Description { get; }
        string FormatActionCount(int actionCount);
    }
}