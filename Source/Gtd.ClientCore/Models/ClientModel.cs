#region (c) 2012-2013 Copyright BeingTheWorst.com Podcast

// See being the worst podcast for more details

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gtd.Client.Models
{
    public sealed class ClientModel
    {
        readonly IMessageQueue _queue;

        ImmutableDictionary<StuffId, ImmutableStuff> _stuffInInbox =
            ImmutableDictionary.Create<StuffId, ImmutableStuff>();

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
            return new ImmutableInbox(_stuffInInbox);
        }


        public int GetTheNumberOfItemsOfStuffInInbox()
        {
            return _stuffInInbox.Count;
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
                GetInbox(),
                ImmutableList.Create(_projectList.Select(m => m.Freeze()).ToArray()));

            Publish(new Dumb.ClientModelLoaded(model));
        }

        void Publish(Dumb.CliendModelEvent e)
        {
            if (!_loadingCompleted)
                return;
            _queue.Enqueue(e);
        }

        uint _stuffOrderCounter;

        public void StuffPutInInbox(StuffId stuffId, string descriptionOfStuff, DateTime date)
        {
            var key = "stuff-" + Id.Id;
            var item = new ImmutableStuff(stuffId, descriptionOfStuff, key, _stuffOrderCounter++);


            _stuffInInbox = _stuffInInbox.Add(stuffId, item);

            Publish(new Dumb.StuffAddedToInbox(item, _stuffInInbox.Count));
        }

        public void StuffTrashed(StuffId stuffId)
        {
            ImmutableStuff value;
            if (!_stuffInInbox.TryGetValue(stuffId, out value))
                return;

            _stuffInInbox = _stuffInInbox.Remove(stuffId);

            Publish(new Dumb.StuffRemovedFromInbox(value, _stuffInInbox.Count));
        }


        public void StuffArchived(StuffId stuffId)
        {
            ImmutableStuff value;
            if (!_stuffInInbox.TryGetValue(stuffId, out value))
                return;


            _stuffInInbox = _stuffInInbox.Remove(stuffId);

            Publish(new Dumb.StuffRemovedFromInbox(value, _stuffInInbox.Count));
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
            var oldStuff = _stuffInInbox[stuffId];


            var newValue = oldStuff.WithDescription(newDescriptionOfStuff);

            _stuffInInbox = _stuffInInbox.SetItem(stuffId, newValue);

            Publish(new Dumb.StuffUpdated(newValue));
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

        public void Verify(TrustedSystemId id) {}

        public void Create(TrustedSystemId id) {}


        sealed class MutableProject
        {
            ProjectId ProjectId { get; set; }
            string Outcome { get; set; }
            ProjectType Type { get; set; }
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
            ActionId Id { get; set; }
            string Outcome { get; set; }
            bool Completed { get; set; }
            bool Archived { get; set; }
            public ProjectId ProjectId { get; private set; }
            DateTime DeferUntil { get; set; }
            DateTime DueDate { get; set; }

            string UIKey
            {
                get { return "action-" + Id.Id; }
            }

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