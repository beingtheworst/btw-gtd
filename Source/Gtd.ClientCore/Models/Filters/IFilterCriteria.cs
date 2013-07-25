using System.Collections.Generic;
using Gtd.Client.Models;


namespace Gtd.Shell.Filters
{
    public interface IFilterCriteria
    {
        IEnumerable<ImmutableAction> FilterActions(ImmutableProject model); 

        string Title { get; }
        string Description { get; }
        string FormatActionCount(int actionCount);
    }
}