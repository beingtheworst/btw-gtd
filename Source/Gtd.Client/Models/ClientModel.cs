using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gtd.Client.Models
{
    public sealed class ClientModel
    {
        readonly IMessageQueue _queue;
        List<MutableStuff> _listOfGtdStuff = new List<MutableStuff>();
        readonly Dictionary<StuffId, MutableStuff> _stuffInInbox = new Dictionary<StuffId, MutableStuff>();

        readonly List<MutableProject> _projectList = new List<MutableProject>();
        readonly Dictionary<ProjectId, MutableProject> _projects = new Dictionary<ProjectId, MutableProject>();
        readonly Dictionary<ActionId, MutableAction> _actions = new Dictionary<ActionId, MutableAction>();
        


        static ImmutableProject Immute(MutableProject m)
        {
            var ma = m.Actions.Select(Immute).ToList().AsReadOnly();
            return new ImmutableProject(m.UIKey, m.ProjectId, m.Outcome, m.Type, ma);
        }

        static ImmutableAction Immute(MutableAction mutable)
        {
            return new ImmutableAction(mutable.UIKey,
                mutable.Id,
                mutable.Outcome,
                mutable.Completed,
                mutable.Archived,
                mutable.ProjectId,
                mutable.DeferUntil,
                mutable.DueDate);
        }



        public IList<ImmutableProject> ListProjects()
        {
            return _projectList.Select(Immute).ToList().AsReadOnly();
        } 

        public ImmutableProject GetProjectOrNull(ProjectId projectId)
        {
            MutableProject value;
            if (_projects.TryGetValue(projectId, out value))
                return Immute(value);
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
                ImmutableList.Create(_projectList.Select(Immute).ToArray()));

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

            Publish(new Dumb.ActionAdded(actionId, action.UIKey, projectId, project.UIKey, outcome));
        }

        public void ActionCompleted(ActionId actionId)
        {
            var action = _actions[actionId];
            var project = _projects[action.ProjectId];
            action.MarkAsCompleted();
            Publish(new Dumb.ActionUpdated(actionId, action.UIKey, action.ProjectId, project.UIKey, action.Outcome, true));
        }

        public void DescriptionOfStuffChanged(StuffId stuffId, string newDescriptionOfStuff)
        {
            _stuffInInbox[stuffId].UpdateDescription(newDescriptionOfStuff);
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
            Publish(new Dumb.ActionUpdated(actionId, action.UIKey, action.ProjectId, project.UIKey, action.Outcome, action.Completed));
        }

        public void ActionArchived(ActionId id)
        {
            _actions[id].MarkAsArchived();
        }

        public void ProjectTypeChanged(ProjectId projectId, ProjectType type)
        {
            _projects[projectId].TypeChanged(type);
        }

        public void DeferredUtil(ActionId actionId, DateTime deferUntil)
        {
            _actions[actionId].DeferUntilDate(deferUntil);
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
            public void OutcomeChanged(string outcome)
            {
                Outcome = outcome;
            }

            public void MarkAsArchived()
            {
                Archived = true;

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