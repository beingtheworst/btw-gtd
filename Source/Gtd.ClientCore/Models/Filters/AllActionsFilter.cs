using System.Collections.Generic;
using Gtd.Client.Models;

namespace Gtd.Shell.Filters
{
    public sealed class AllActionsFilter : IFilterCriteria
    {
        public IEnumerable<ImmutableAction> FilterActions(ImmutableProject model)
        {
            return model.Actions;
        }

        public string Title { get { return "All actions"; } }
        public string Description { get { return "includes completed and archived items"; } }

        public string FormatActionCount(int actionCount)
        {
            return string.Format("{0} total", actionCount);
        }
    }
}