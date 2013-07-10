using System;
using System.Collections.Generic;
using Gtd.Shell.Filters;

namespace Gtd.Client.Models
{
    

    public sealed class FilterService
    {
        public readonly List<IFilterCriteria> Filters = new List<IFilterCriteria>(); 
        public FilterService()
        {
            Filters.AddRange(FilterCriteria.LoadAllFilters());

        }
    }


    public interface IItemView
    {
        string GetTitle();
    }

    public sealed class ThoughtView : IItemView
    {
        public ThoughtId Id;
        public string Subject;
        public DateTime Added;

        public string GetTitle()
        {
            return string.Format("Thought '{0}'", Subject);
        }

    }

    public sealed class ProjectView : IItemView
    {
        public ProjectId ProjectId { get; private set; }
        public string Outcome { get; private set; }
        public ProjectType Type { get; private set; }

        public ProjectView(ProjectId projectId, string outcome, ProjectType type)
        {
            ProjectId = projectId;
            Outcome = outcome;
            Type = type;
        }


        public List<ActionView> Actions = new List<ActionView>();

        public void OutcomeChanged(string outcome)
        {
            Outcome = outcome;
        }

        public void TypeChanged(ProjectType type)
        {
            Type = type;
        }

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
        public DateTime StartDate { get; private set; }
        public DateTime DueDate { get; private set; }
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
        public void OutcomeChanged(string outcome)
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

        public void StartDateAssigned(DateTime newStartDate)
        {
            StartDate = newStartDate;
        }
        public void DueDateAssigned(DateTime newDueDate)
        {
            DueDate = newDueDate;
        }
    }

    
    public sealed class ClientModel
    {
        public List<ThoughtView> Thoughts = new List<ThoughtView>();
        public List<ProjectView> ProjectList = new List<ProjectView>();
        public Dictionary<ProjectId, ProjectView> ProjectDict = new Dictionary<ProjectId, ProjectView>();
        public Dictionary<ActionId, ActionView> ActionDict = new Dictionary<ActionId, ActionView>();
        public Dictionary<Guid, IItemView> DictOfAllItems = new Dictionary<Guid, IItemView>();


        

        public TrustedSystemId Id { get; private set; }

        public void ThoughtCaptured(ThoughtId thoughtId, string thought, DateTime date)
        {
            var item = new ThoughtView()
            {
                Added = date,
                Id = thoughtId,
                Subject = thought
            };
            Thoughts.Add(item);
            DictOfAllItems.Add(thoughtId.Id, item);
        }

        public void ThoughtArchived(ThoughtId thoughtId)
        {
            Thoughts.RemoveAll(t => t.Id == thoughtId);
        }


        public void ProjectDefined(ProjectId projectId, string projectOutcome, ProjectType type)
        {
            var project = new ProjectView(projectId, projectOutcome, type);
            ProjectList.Add(project);
            ProjectDict.Add(projectId, project);
            DictOfAllItems.Add(projectId.Id, project);
        }

        public void ActionDefined(ProjectId projectId, ActionId actionId, string outcome)
        {
            var action = new ActionView(actionId, outcome, projectId);

            ProjectDict[projectId].Actions.Add(action);
            ActionDict.Add(actionId, action);
            DictOfAllItems.Add(actionId.Id, action);
        }
        public void ActionCompleted(ActionId actionId)
        {
            ActionDict[actionId].MarkAsCompleted();
        }

        public void ThoughtSubjectChanged(ThoughtId thoughtId, string subject)
        {
            ((ThoughtView)DictOfAllItems[thoughtId.Id]).Subject = subject;
        }
        public void ProjectOutcomeChanged(ProjectId projectId, string outcome)
        {
            ProjectDict[projectId].OutcomeChanged(outcome);
        }
        public void ActionOutcomeChanged(ActionId actionId, string outcome)
        {
            ActionDict[actionId].OutcomeChanged(outcome);
        }

        public void ActionArchived(ActionId id)
        {
            ActionDict[id].MarkAsArchived();
        }

        public void ProjectTypeChanged(ProjectId projectId, ProjectType type)
        {
            ProjectDict[projectId].TypeChanged(type);
        }

        public void StartDateAssigned(ActionId actionId, DateTime newStartDate)
        {
            ActionDict[actionId].StartDateAssigned(newStartDate);
        }
        public void DueDateAssigned(ActionId actionId, DateTime newDueDate)
        {
            ActionDict[actionId].DueDateAssigned(newDueDate);
        }




        
        public void Create(TrustedSystemId id)
        {
            Id = id;
        }

        public void Verify(TrustedSystemId id)
        {

            //if (_currentId.Id != id.Id)
            //    throw new InvalidOperationException();
        }
    }
}