using System;
using System.Linq;
using System.Reflection;
using Gtd.Shell.Projections;

namespace Gtd.Shell
{
    public interface IFilterCriteria
    {
        bool IncludeAction(ProjectView project,  ActionView action);
        string Title { get; }
        string Description { get; }
        string FormatActionCount(int actionCount);
    }
}