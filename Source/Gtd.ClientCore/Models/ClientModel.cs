using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gtd.Client.Models
{
    public sealed class ClientModel
    {
        readonly IMessageQueue _queue;
        readonly List<MutableStuff> _listOfGtdStuff = new List<MutableStuff>();
        readonly Dictionary<StuffId, MutableStuff> _stuffInInbox = new Dictionary<StuffId, MutableStuff>();

        readonly List<MutableProject> _projectList = new List<MutableProject>();
        readonly Dictionary<ProjectId, MutableProject> _projects = new Dictionary<ProjectId, MutableProject>();
        readonly Dictionary<ActionId, MutableAction> _actions = new Dictionary<ActionId, MutableAction>();


        public IList<ImmutableProject> ListProjects()
        {
            return _projectList.Select(m => m.Freeze()).ToList().AsReadOnly();
        } 

        public ImmutableProject GetProjectOrNull(ProjectId projectId)
        {
            MutableProject value;
            if (_projects.TryGetValue(projectId, out value))
                return value.Freeze();
            return null;
        }

        public ImmutableInbox GetInbox()
        {
            return new ImmutableInbox(ImmutableList.Create(_listOfGtdStuff.Select(f => f.Freeze()).ToArray()));
        }

        public int GetTheNumberOfItemsOfStuffInInbox()
        {
            return _listOfGtdStuff.Count;
        }

        public readonly TrustedSystemId Id;

        public ClientModel(IMessageQueue queue, TrustedSystemId id)
        {
            _queue = queue;
            Id = id;
        }

        bool _loadingCompleted;

        public void LoadingCompleted()
        {
            _loadingCompleted = true;

            var model = new ImmutableClientModel(
                ImmutableList.Create(_listOfGtdStuff.Select(f => f.Freeze()).ToArray()),
                ImmutableList.Create(_projectList.Select(m => m.Freeze()).ToArray()));

            Publish(new Dumb.ClientModelLoaded(model));
        }

        void Publish(Dumb.CliendModelEvent e)
        {
            if (!_loadingCompleted)
                return;
            _queue.Enqueue(e);
        }

        public void StuffPutInInbox(StuffId stuffId, string descriptionOfStuff, DateTime date)
        {
            var key = "stuff-" + Id.Id;
            var item = new MutableStuff(stuffId, descriptionOfStuff, key);

            _listOfGtdStuff.Add(item);
            _stuffInInbox.Add(stuffId, item);
            Publish(new Dumb.StuffAddedToInbox(item.Freeze(), _listOfGtdStuff.Count));
        }

        public void StuffTrashed(StuffId stuffId)
        {
            MutableStuff value;
            if (!_stuffInInbox.TryGetValue(stuffId, out value))
                return;

            _listOfGtdStuff.Remove(value);

            Publish(new Dumb.StuffRemovedFromInbox(value.Freeze(), _listOfGtdStuff.Count));
        }


        public void StuffArchived(StuffId stuffId)
        {
            MutableStuff value;
            if (!_stuffInInbox.TryGetValue(stuffId, out value))
                return;

            _listOfGtdStuff.Remove(value);

            Publish(new Dumb.StuffRemovedFromInbox(value.Freeze(), _listOfGtdStuff.Count));
        }

        public void ProjectDefined(ProjectId projectId, string projectOutcome, ProjectType type)
        {
            var project = new MutableProject(projectId, projectOutcome, type);
            _projectList.Add(project);
            _projects.Add(projectId, project);
            

            Publish(new Dumb.ProjectAdded(project.UIKey, projectOutcome, projectId));
        }

        public void ActionDefined(ProjectId projectId, ActionId actionId, string outcome)
        {
            var action = new MutableAction(actionId, outcome, projectId);

            var project = _projects[projectId];
            project.Actions.Add(action);
            _actions.Add(actionId, action);

            Publish(new Dumb.ActionAdded(action.Freeze()));
        }
        public void ActionMoved(ActionId actionId, ProjectId oldProjectId, ProjectId newProjectId)
        {
            var action = _actions[actionId];
            Publish(new Dumb.ActionRemoved(action.Freeze()));

            action.MoveToProject(newProjectId);

            Publish(new Dumb.ActionAdded(action.Freeze()));

            _projects[oldProjectId].Actions.Remove(action);
            _projects[newProjectId].Actions.Add(action);
        }

        public void ActionCompleted(ActionId actionId)
        {
            var action = _actions[actionId];
            var project = _projects[action.ProjectId];
            action.MarkAsCompleted();
            
            Publish(new Dumb.ActionUpdated(action.Freeze(), project.UIKey));
        }

        public void DescriptionOfStuffChanged(StuffId stuffId, string newDescriptionOfStuff)
        {
            var stuff = _stuffInInbox[stuffId];
            stuff.UpdateDescription(newDescriptionOfStuff);

            Publish(new Dumb.StuffUpdated(stuff.Freeze()));
        }

        public void ProjectOutcomeChanged(ProjectId projectId, string outcome)
        {
            _projects[projectId].OutcomeChanged(outcome);
        }

        public void ActionOutcomeChanged(ActionId actionId, string outcome)
        {
            var action = _actions[actionId];
            var project = _projects[action.ProjectId];
            action.OutcomeChanged(outcome);
            var immutable = action.Freeze();
            Publish(new Dumb.ActionUpdated(immutable, project.UIKey));
        }

        public void ActionArchived(ActionId id)
        {
            var action = _actions[id];
            action.MarkAsArchived();

            Publish(new Dumb.ActionAdded(action.Freeze()));
        }

        public void ProjectTypeChanged(ProjectId projectId, ProjectType type)
        {
            _projects[projectId].TypeChanged(type);
        }

        public void DeferredUtil(ActionId actionId, DateTime deferUntil)
        {
            var action = _actions[actionId];
            action.DeferUntilDate(deferUntil);
        }

        public void DueDateAssigned(ActionId actionId, DateTime newDueDate)
        {
            _actions[actionId].DueDateAssigned(newDueDate);
        }

        public void Verify(TrustedSystemId id)
        {
            
        }

        public void Create(TrustedSystemId id)
        {
            
        }

        sealed class MutableStuff
        {
            public StuffId StuffId { get; private set; }
            public string Description { get; private set; }
            public  string UIKey { get; private set; }

            public MutableStuff(StuffId stuffId, string description, string uiKey)
            {
                StuffId = stuffId;
                Description = description;
                UIKey = uiKey;
            }


            public void UpdateDescription(string newDescriptionOfStuff)
            {
                Description = newDescriptionOfStuff;
            }

            public ImmutableStuff Freeze()
            {
                return new ImmutableStuff(StuffId, Description, UIKey);
            }
        }

        sealed class MutableProject 
        {
            public ProjectId ProjectId { get; private set; }
            public string Outcome { get; private set; }
            public ProjectType Type { get; private set; }
            public readonly string UIKey;

            public MutableProject(ProjectId projectId, string outcome, ProjectType type)
            {
                ProjectId = projectId;
                Outcome = outcome;
                Type = type;

                UIKey = "project-" + projectId.Id;
            }

            public ImmutableProject Freeze()
            {
                var ma = Actions.Select(mutable => mutable.Freeze()).ToList().AsReadOnly();
                var info = new ImmutableProjectInfo(ProjectId, Outcome, Type, UIKey);
                return new ImmutableProject(info, ma);

            }


            public readonly List<MutableAction> Actions = new List<MutableAction>();

            public void OutcomeChanged(string outcome)
            {
                Outcome = outcome;
            }

            public void TypeChanged(ProjectType type)
            {
                Type = type;
            }
        }

        sealed class MutableAction 
        {
            public ActionId Id { get; private set; }
            public string Outcome { get; private set; }
            public bool Completed { get; private set; }
            public bool Archived { get; private set; }
            public ProjectId ProjectId { get; private set; }
            public DateTime DeferUntil { get; private set; }
            public DateTime DueDate { get; private set; }

            public string UIKey { get { return "action-" + Id.Id; } }

            public MutableAction(ActionId action, string outcome, ProjectId project)
            {
                Id = action;
                Outcome = outcome;
                Completed = false;
                Archived = false;
                ProjectId = project;
            }

            public void MarkAsCompleted()
            {
                Completed = true;
            }
            public void MoveToProject(ProjectId newProject)
            {
                ProjectId = newProject;
            }
            public void OutcomeChanged(string outcome)
            {
                Outcome = outcome;
            }

            public void MarkAsArchived()
            {
                Archived = true;
            }

            public ImmutableAction Freeze()
            {
                return new ImmutableAction(UIKey,
                    Id,
                    Outcome,
                    Completed,
                    Archived,
                    ProjectId,
                    DeferUntil,
                    DueDate);
            }

            public void DeferUntilDate(DateTime newStartDate)
            {
                DeferUntil = newStartDate;
            }
            public void DueDateAssigned(DateTime newDueDate)
            {
                DueDate = newDueDate;
            }
        }
    }
}