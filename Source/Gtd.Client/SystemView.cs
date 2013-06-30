using System.Collections.Generic;
using System.Linq;

namespace Gtd.Client
{
    public class SystemView : IHandle<ProjectDefined>, ISystemView
    {



        public void Subscribe(ISubscriber bus)
        {
            bus.Subscribe<ProjectDefined>(this);
        }

        public int Version { get; private set; }

        IDictionary<ProjectId, ProjectInfo> _projectDict = new Dictionary<ProjectId, ProjectInfo>();
        IList<ProjectInfo> _projects = new List<ProjectInfo>();

        public void Handle(ProjectDefined message)
        {
            var pi = new ProjectInfo(message.ProjectId, message.ProjectOutcome);
            _projects.Add(pi);
            _projectDict.Add(pi.Id, pi);

            Version += 1;
        }

        public IList<ProjectInfo> ListProjects()
        {
            return _projects.ToArray();
        }
    }

    public class ProjectInfo
    {
        public readonly ProjectId Id;
        public readonly string Outcome;



        public ProjectInfo(ProjectId id, string outcome)
        {
            Id = id;
            Outcome = outcome;
        }
    }


    public interface ISystemView
    {
        int Version { get; }
        IList<ProjectInfo> ListProjects();
    }
}