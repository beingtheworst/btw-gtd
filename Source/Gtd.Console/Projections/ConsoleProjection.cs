using System;
using System.Collections.Generic;
using System.Linq;

namespace Gtd.Shell.Projections
{
    public sealed class ConsoleView
    {
        public IDictionary<TrustedSystemId, TrustedSystem> Systems = new Dictionary<TrustedSystemId, TrustedSystem>();
    }

    public interface IItemView
    {
        string GetTitle();
    }

    public sealed class ThoughtView : IItemView
    {
        public Guid Id;
        public string Subject;
        public DateTime Added;

        public string GetTitle()
        {
            return string.Format("Thought '{0}'", Subject);
        }

    }

    public sealed class ProjectView : IItemView
    {
        public ProjectId ProjectId;
        public string Outcome;

        public List<ActionView> ActiveActions = new List<ActionView>();

        public string GetTitle()
        {
            return string.Format("Project '{0}'", Outcome);
        }
    }

    public sealed class ActionView : IItemView
    {
        public ActionId Id { get; private set; }
        public string Outcome { get; private set; }
        public bool Completed { get; private set; }
        public bool Archived { get; private set; }
        public ProjectId Project { get; private set; }

        public ActionView(ActionId action, string outcome, ProjectId project)
        {
            Id = action;
            Outcome = outcome;
            Completed = false;
            Archived = false;
            Project = project;
        }

        public void MarkAsCompleted()
        {
            Completed = true;
        }
        public void ChangeOutcome(string outcome)
        {
            Outcome = outcome;
        }

        public string GetTitle()
        {
            return string.Format("Action: '{0}'", Outcome);
        }

        public void MarkAsArchived()
        {
            Archived = true;

        }
    }

    public sealed class TrustedSystem
    {
        public List<ThoughtView> Thoughts = new List<ThoughtView>(); 
        public List<ProjectView> ProjectList = new List<ProjectView>(); 
        public Dictionary<ProjectId, ProjectView> ProjectDict = new Dictionary<ProjectId, ProjectView>(); 
        public Dictionary<ActionId, ActionView> ActionDict = new Dictionary<ActionId, ActionView>(); 

        public Dictionary<Guid, IItemView> GlobalDict = new Dictionary<Guid, IItemView>();
       
        public void CaptureThought(Guid thoughtId, string thought, DateTime date)
        {
            var item = new ThoughtView()
                {
                    Added = date, Id = thoughtId, Subject = thought
                };
            Thoughts.Add(item);
            GlobalDict.Add(thoughtId, item);
        }

        public void ArchiveThought(Guid thoughtId)
        {
            Thoughts.RemoveAll(t => t.Id == thoughtId);
        }

       
        public void DefineProject(ProjectId projectId, string projectOutcome)
        {
            var project = new ProjectView
                {
                    ProjectId = projectId, Outcome = projectOutcome
                };
            ProjectList.Add(project);
            ProjectDict.Add(projectId, project);
            GlobalDict.Add(projectId.Id, project);
        }

        public void DefineAction(ProjectId projectId, ActionId actionId, string outcome)
        {
            var action = new ActionView(actionId, outcome, projectId);

            ProjectDict[projectId].ActiveActions.Add(action);
            ActionDict.Add(actionId, action);
            GlobalDict.Add(actionId.Id, action);
        }
        public void CompleteAction(ActionId actionId)
        {
            ActionDict[actionId].MarkAsCompleted();
        }

        public void ChangeThoughtSubject(Guid thoughtId, string subject)
        {
            ((ThoughtView) GlobalDict[thoughtId]).Subject = subject;
        }
        public void ChangeProjectOutcome(ProjectId projectId, string outcome)
        {
            ProjectDict[projectId].Outcome = outcome;
        }
        public void ChangeActionOutcome(ActionId actionId, string outcome)
        {
            ActionDict[actionId].ChangeOutcome(outcome);
        }

        public void ArchiveAction(ActionId id)
        {
            var view = ActionDict[id];
            view.MarkAsArchived();
            ProjectDict[view.Project].ActiveActions.Remove(view);
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
        public void When(ActionCompleted evnt)
        {
            Update(evnt.Id, s => s.CompleteAction(evnt.ActionId));
        }
        public void When(ActionOutcomeChanged evnt)
        {
            Update(evnt.Id, s => s.ChangeActionOutcome(evnt.ActionId, evnt.ActionOutcome));
        }

        public void When(ProjectOutcomeChanged evnt)
        {
            Update(evnt.Id, s => s.ChangeProjectOutcome(evnt.ProjectId, evnt.ProjectOutcome));
        }

        public void When(ThoughtSubjectChanged evnt)
        {
            Update(evnt.Id, s => s.ChangeThoughtSubject(evnt.ThoughtId, evnt.Subject));
        }
        public void When(ActionArchived evnt)
        {
            Update(evnt.Id, s => s.ArchiveAction(evnt.ActionId));
        }
    }
}