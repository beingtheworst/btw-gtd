using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    public interface IFilterCriteria
    {
        bool IncludeAction(ProjectView project,  ActionView action);
        string Title { get; }
    }

    public static class FilterCriteria
    {
                

        public static IEnumerable<IFilterCriteria>  LoadALlFilters()
        {
            var consoleCommandTypes = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => typeof(IFilterCriteria).IsAssignableFrom(t))
                .Where(t => !t.IsAbstract)
                .ToArray();


            foreach (var type in consoleCommandTypes)
            {
                var instance = (IFilterCriteria) Activator.CreateInstance(type);

                yield return instance;
            }
        }

    }

    public sealed class AllRemaining : IFilterCriteria
    {
        public bool IncludeAction(ProjectView proect, ActionView view)
        {
            if (view.Archived)
                return false;
            if (view.Completed)
                return false;

            return true;
        }

        public string Title { get { return "All remaining actions (non-complete, non-archived)"; } }
    }

    public sealed class AllActions : IFilterCriteria
    {
        public bool IncludeAction(ProjectView project, ActionView action)
        {
            return true;
        }

        public string Title { get { return "All actions"; } }
    }

}