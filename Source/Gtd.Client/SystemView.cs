using System;
using System.Collections.Generic;
using System.Linq;
using Gtd.CoreDomain;
using Gtd.Shell.Filters;

namespace Gtd.Client
{
    public interface ISystemView
    {
        IList<ProjectView> ListProjects();
        ThoughtView[] ListInbox();
        ProjectView GetProjectOrNull(ProjectId id);
    }

    public sealed class FilterService
    {
        public readonly List<IFilterCriteria> Filters = new List<IFilterCriteria>(); 
        public FilterService()
        {
            Filters.AddRange(FilterCriteria.LoadAllFilters());

        }
    }

    public sealed class SystemView 
    {
        public IDictionary<TrustedSystemId, TrustedSystem> Systems = new Dictionary<TrustedSystemId, TrustedSystem>();
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

    // This is an in-memory View that the Projection class is updating the state of.
    // In a production system, Projection classes tend to be updating state
    // to a persistent disk (file, Redis, etc.) or into some other system 
    // rather than in-memory.  May also be broken down into smaller Views so that we
    // don't just throw around one big/large object.
    // This in-memory View is currently loaded in almost every Console command that
    // displays information to the shell/console screen.
    // (ex: Gtd.Shell.Commands.ListActionsCommand)
    public sealed class TrustedSystem
    {
        public List<ThoughtView> Thoughts = new List<ThoughtView>();
        public List<ProjectView> ProjectList = new List<ProjectView>();
        public Dictionary<ProjectId, ProjectView> ProjectDict = new Dictionary<ProjectId, ProjectView>();
        public Dictionary<ActionId, ActionView> ActionDict = new Dictionary<ActionId, ActionView>();
        public Dictionary<Guid, IItemView> DictOfAllItems = new Dictionary<Guid, IItemView>();

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
    }


    public sealed class SystemProjection :
        IHandle<TrustedSystemCreated>,
        IHandle<ThoughtCaptured>,
        IHandle<ThoughtArchived>,
        IHandle<ProjectDefined>,
        IHandle<ActionDefined>,
        IHandle<FormLoading>,
        IHandle<ActionCompleted>,
    ISystemView

    {
        readonly IEventStore _store;

        public SystemProjection(IEventStore store)
        {
            _store = store;
        }

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<TrustedSystemCreated>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
            bus.Subscribe<ProjectDefined>(this);
            bus.Subscribe<ActionDefined>(this);
            bus.Subscribe<FormLoading>(this);
            bus.Subscribe<ActionCompleted>(this);
        }

        public SystemView ViewInstance = new SystemView();

        public ProjectView GetProjectOrNull(ProjectId id)
        {
            return GetCurrentSystem().ProjectDict[id];
        }

        public IList<ProjectView> ListProjects()
        {
            return GetCurrentSystem().ProjectList.ToArray();
        }

        public TrustedSystem GetCurrentSystem()
        {
            var system = ViewInstance.Systems.Select(v => v.Value).FirstOrDefault();
            if (null == system)
                throw new InvalidOperationException("System should be available");
            return system;
        }

        public ThoughtView[] ListInbox()
        {

            return GetCurrentSystem().Thoughts.ToArray();
        }

        void Update(TrustedSystemId id, Action<TrustedSystem> update)
        {
            update(ViewInstance.Systems[id]);
        }

        public void Handle(TrustedSystemCreated e)
        {
            ViewInstance.Systems.Add(e.Id, new TrustedSystem());
        }

        public void Handle(ThoughtCaptured e)
        {
            Update(e.Id, s => s.ThoughtCaptured(e.ThoughtId, e.Thought, e.TimeUtc));
        }
        public void Handle(ThoughtArchived e)
        {
            Update(e.Id, s => s.ThoughtArchived(e.ThoughtId));
        }

        public void Handle(ProjectDefined e)
        {
            Update(e.Id, s => s.ProjectDefined(e.ProjectId, e.ProjectOutcome, e.Type));
        }
        public void Handle(ActionDefined e)
        {
            Update(e.Id, s => s.ActionDefined(e.ProjectId, e.ActionId, e.Outcome));
        }
        public void Handle(ActionCompleted e)
        {
            Update(e.Id, s => s.ActionCompleted(e.ActionId));
        }
        public void Handle(ActionOutcomeChanged e)
        {
            Update(e.Id, s => s.ActionOutcomeChanged(e.ActionId, e.ActionOutcome));
        }

        public void Handle(ProjectOutcomeChanged e)
        {
            Update(e.Id, s => s.ProjectOutcomeChanged(e.ProjectId, e.ProjectOutcome));
        }

        public void Handle(ThoughtSubjectChanged e)
        {
            Update(e.Id, s => s.ThoughtSubjectChanged(e.ThoughtId, e.Subject));
        }
        public void Handle(ActionArchived e)
        {
            Update(e.Id, s => s.ActionArchived(e.ActionId));
        }
        public void Handle(ProjectTypeChanged e)
        {
            Update(e.Id, s => s.ProjectTypeChanged(e.ProjectId, e.Type));
        }
        public void Handle(StartDateAssignedToAction e)
        {
            Update(e.Id, s => s.StartDateAssigned(e.ActionId, e.NewStartDate));
        }
        public void Handle(DueDateAssignedToAction e)
        {
            Update(e.Id, s => s.DueDateAssigned(e.ActionId, e.NewDueDate));
        }
        public void Handle(StartDateRemovedFromAction e)
        {
            Update(e.Id, s => s.StartDateAssigned(e.ActionId, DateTime.MinValue));
        }
        public void Handle(DueDateRemovedFromAction e)
        {
            Update(e.Id, s => s.DueDateAssigned(e.ActionId, DateTime.MinValue));
        }
        public void Handle(ActionStartDateMoved e)
        {
            Update(e.Id, s => s.StartDateAssigned(e.ActionId, e.NewStartDate));
        }
        public void Handle(ActionDueDateMoved e)
        {
            Update(e.Id, s => s.DueDateAssigned(e.ActionId, e.NewDueDate));
        }

        public void Handle(FormLoading _)
        {
            foreach (var e in _store.LoadEventStream("app").Events)
            {
                ((dynamic) this).Handle((dynamic) e);
            }
        }
    }
}