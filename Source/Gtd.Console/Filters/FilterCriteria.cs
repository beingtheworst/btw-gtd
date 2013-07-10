using System.Collections.Generic;

namespace Gtd.Shell.Filters
{
    public static class FilterCriteria
    {
        public static IEnumerable<IFilterCriteria>  LoadAllFilters()
        {
            yield return new RemainingFilter();
            yield return new AvailableFilter();
            yield return new NextActionFilter();
            yield return new AllActionsFilter();
        }

    }
}