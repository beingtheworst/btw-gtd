using System;
using System.Collections.Generic;
using Gtd.Client.Models;
using System.Linq;

namespace Gtd.Shell.Filters
{
    public sealed class NextActionFilter : IFilterCriteria
    {

        public IEnumerable<ImmutableAction> FilterActions(ImmutableProject model)
        {
            if (model.Type == ProjectType.List)
            {
                // in list, every available action is next
                foreach (var actionView in GetAllAvailableActions(model))
                {
                    yield return actionView;
                }
            }
            else if (model.Type == ProjectType.Parallel)
            {
                // in parallel, first available action is next
                var filtered = GetAllAvailableActions(model).FirstOrDefault();

                if (filtered != null)
                {
                    yield return filtered;
                }
            }
            else
            {
                // in sequential, first available action is next (unless it's blocked)
                var filtered = model.Actions
                                   .Where(v => !v.Completed)
                                   .Where(v => !v.Archived)
                                   .FirstOrDefault();

                if (filtered != null && filtered.StartDate <= DateTime.UtcNow)
                    yield return filtered;

            }

        }

        static IEnumerable<ImmutableAction> GetAllAvailableActions(ImmutableProject model)
        {
            return model.Actions
                       .Where(v => !v.Completed)
                       .Where(v => !v.Archived)
                       .Where(v => v.StartDate <= DateTime.Now);
        }

        public string Title
        {
            get { return "Next"; }
        }

        public string Description
        {
            get { return "next thing to do in each eligible project"; }
        }

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