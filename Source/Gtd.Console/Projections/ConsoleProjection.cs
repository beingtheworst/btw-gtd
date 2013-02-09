using System;
using System.Collections.Generic;
using System.Linq;

namespace Gtd.Shell.Projections
{
    public sealed class ConsoleView
    {
        public IDictionary<TrustedSystemId, TrustedSystem> Systems = new Dictionary<TrustedSystemId, TrustedSystem>();
    }

    public sealed class Thought
    {
        public Guid Id;
        public string Subject;
        public DateTime Added;
    }

    public sealed class Project
    {
        public ProjectId ProjectId;
        public string Outcome;

        public List<Action> Actions = new List<Action>(); 

        public void AddAction(ActionId actionId, string outcome)
        {
            Actions.Add(new Action(actionId, outcome));
        }
    }

    public sealed class Action
    {
        public ActionId Id { get; private set; }
        public string Outcome { get; private set; }

        public Action(ActionId action, string outcome)
        {
            Id = action;
            Outcome = outcome;
        }
    }

    public sealed class TrustedSystem
    {
        public List<Thought> Thoughts = new List<Thought>(); 
        public List<Project> ProjectList = new List<Project>(); 
        public Dictionary<ProjectId, Project> ProjectDict = new Dictionary<ProjectId, Project>(); 

       
        public void CaptureThought(Guid thoughtId, string thought, DateTime date)
        {
            Thoughts.Add(new Thought()
                {
                    Added = date,
                    Id = thoughtId,
                    Subject = thought
                });
        }
        public void ArchiveThought(Guid thoughtId)
        {
            Thoughts.RemoveAll(t => t.Id == thoughtId);
        }

       
        public void DefineProject(ProjectId projectId, string projectOutcome)
        {
            var project = new Project
                {
                    ProjectId = projectId, Outcome = projectOutcome
                };
            ProjectList.Add(project);
            ProjectDict.Add(projectId, project);
        }

        public void DefineAction(ProjectId projectId, ActionId actionId, string outcome)
        {
            ProjectDict[projectId].AddAction(actionId, outcome);
        }
    }

    public sealed class ConsoleProjection
    {
        public ConsoleView ViewInstance = new ConsoleView();

        void Update(TrustedSystemId id, Action<TrustedSystem> update)
        {
            update(ViewInstance.Systems[id]);
        }

        public void When(TrustedSystemCreated evnt)
        {
            ViewInstance.Systems.Add(evnt.Id, new TrustedSystem());
        }

        public void When(ThoughtCaptured evnt)
        {
            Update(evnt.Id, s => s.CaptureThought(evnt.ThoughtId, evnt.Thought, evnt.TimeUtc));
        }
        public void When(ThoughtArchived evnt)
        {
            Update(evnt.Id, s => s.ArchiveThought(evnt.ThoughtId));
        }

        public void When(ProjectDefined evnt)
        {
            Update(evnt.Id, s => s.DefineProject(evnt.ProjectId, evnt.ProjectOutcome));
        }
        public void When(ActionDefined evnt)
        {
            Update(evnt.Id, s => s.DefineAction(evnt.ProjectId, evnt.ActionId, evnt.Outcome));
        }
    }
}