using System.Collections.Generic;
using Gtd.Client.Models;


namespace Gtd.Shell.Filters
{
    public interface IFilterCriteria
    {
        IEnumerable<ActionModel> FilterActions(ProjectModel model); 

        string Title { get; }
        string Description { get; }
        string FormatActionCount(int actionCount);
    }
}