using System;
using System.Collections.Generic;
using Gtd.Shell.Filters;
using System.Linq;

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


    


    
    public sealed class ClientModel
    {
        readonly IMessageQueue _queue;
        readonly List<MutableThought> _thoughts = new List<MutableThought>();
        readonly List<MutableProject> _projectList = new List<MutableProject>();
        readonly Dictionary<ProjectId, MutableProject> _projectDict = new Dictionary<ProjectId, MutableProject>();
        readonly Dictionary<ActionId, MutableAction> _actionDict = new Dictionary<ActionId, MutableAction>();
        readonly Dictionary<Guid, IItemModel> _dictOfAllItems = new Dictionary<Guid, IItemModel>();


        interface IItemModel
        {
            string GetTitle();
        }

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
                mutable.StartDate,
                mutable.DueDate);
        }

        public IList<ImmutableProject> ListProjects()
        {
            return _projectList.Select(Immute).ToList().AsReadOnly();
        } 

        public ImmutableProject GetProjectOrNull(ProjectId id)
        {
            MutableProject value;
            if (_projectDict.TryGetValue(id, out value))
                return Immute(value);
            return null;
        }

        public ImmutableInbox GetInbox()
        {
            var thoughts = _thoughts.Select(t => new ImmutableThought(t.Id, t.Subject, t.UIKey)).ToList().AsReadOnly();
            return new ImmutableInbox(thoughts);
        }
        public int GetNumberOfThoughtsInInbox()
        {
            return _thoughts.Count;
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

            Publish(new Dumb.ClientModelLoaded());
        }

        void Publish(Dumb.CliendModelEvent e)
        {
            if (!_loadingCompleted)
                return;
            _queue.Enqueue(e);
        }

        public void ThoughtCaptured(ThoughtId thoughtId, string thought, DateTime date)
        {
            var item = new MutableThought(thoughtId, thought, date);
            _thoughts.Add(item);
            _dictOfAllItems.Add(thoughtId.Id, item);
            Publish(new Dumb.ThoughtAdded(item.Id, item.Subject, item.UIKey));
        }

        public void ThoughtArchived(ThoughtId thoughtId)
        {

             var thought = this._thoughts.SingleOrDefault(t => t.Id == thoughtId);
            if (null == thought)
                return;

            _thoughts.Remove(thought);

            Publish(new Dumb.ThoughtRemoved(thoughtId,thought.UIKey));
        }


        public void ProjectDefined(ProjectId projectId, string projectOutcome, ProjectType type)
        {
            var project = new MutableProject(projectId, projectOutcome, type);
            _projectList.Add(project);
            _projectDict.Add(projectId, project);
            _dictOfAllItems.Add(projectId.Id, project);

            Publish(new Dumb.ProjectAdded(project.UIKey, projectOutcome, projectId));
        }

        public void ActionDefined(ProjectId projectId, ActionId actionId, string outcome)
        {
            var action = new MutableAction(actionId, outcome, projectId);

            var project = _projectDict[projectId];
            project.Actions.Add(action);
            _actionDict.Add(actionId, action);
            _dictOfAllItems.Add(actionId.Id, action);

            Publish(new Dumb.ActionAdded(actionId, action.UIKey, projectId, project.UIKey, outcome));
        }
        public void ActionCompleted(ActionId actionId)
        {
            var action = _actionDict[actionId];
            var project = _projectDict[action.ProjectId];
            action.MarkAsCompleted();
            Publish(new Dumb.ActionUpdated(actionId, action.UIKey, action.ProjectId, project.UIKey, action.Outcome, true));
        }

        public void ThoughtSubjectChanged(ThoughtId thoughtId, string subject)
        {
            ((MutableThought)_dictOfAllItems[thoughtId.Id]).UpdateSubject(subject);
        }
        public void ProjectOutcomeChanged(ProjectId projectId, string outcome)
        {
            _projectDict[projectId].OutcomeChanged(outcome);
        }
        public void ActionOutcomeChanged(ActionId actionId, string outcome)
        {
            _actionDict[actionId].OutcomeChanged(outcome);
        }

        public void ActionArchived(ActionId id)
        {
            _actionDict[id].MarkAsArchived();
        }

        public void ProjectTypeChanged(ProjectId projectId, ProjectType type)
        {
            _projectDict[projectId].TypeChanged(type);
        }

        public void DeferredUtil(ActionId actionId, DateTime newStartDate)
        {
            _actionDict[actionId].StartDateAssigned(newStartDate);
        }
        public void DueDateAssigned(ActionId actionId, DateTime newDueDate)
        {
            _actionDict[actionId].DueDateAssigned(newDueDate);
        }




        
        

        public void Verify(TrustedSystemId id)
        {
            if (Id.Id != id.Id)
                throw new InvalidOperationException();
        }

        public void Create(TrustedSystemId id)
        {
            
        }




        sealed class MutableThought : IItemModel
        {
            public readonly ThoughtId Id;
            public string Subject { get; private set; }
            public readonly DateTime Added;

            public void UpdateSubject(string subject)
            {
                Subject = subject;
            }

            public string UIKey { get { return "thought-" + Id.Id; } }

            public MutableThought(ThoughtId id, string subject, DateTime added)
            {
                Id = id;
                Subject = subject;
                Added = added;
            }

            public string GetTitle()
            {
                return string.Format("Thought '{0}'", Subject);
            }

        }

         sealed class MutableProject : IItemModel
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

            public string GetTitle()
            {
                return string.Format("Project '{0}'", Outcome);
            }
        }

        sealed class MutableAction : IItemModel
        {
            public ActionId Id { get; private set; }
            public string Outcome { get; private set; }
            public bool Completed { get; private set; }
            public bool Archived { get; private set; }
            public ProjectId ProjectId { get; private set; }
            public DateTime StartDate { get; private set; }
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


    }

    

}