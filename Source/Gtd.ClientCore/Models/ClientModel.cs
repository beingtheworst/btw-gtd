#region (c) 2012-2013 Copyright BeingTheWorst.com Podcast

// See being the worst podcast for more details

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Gtd.Client.Models
{

    /// <summary> This is part of the "Client Model" or "UI" Context
    /// This context is similar to a read model but it also has certain behaviors.
    /// You can perceive this "client model context" as an Aggregate that reacts to 
    /// Events that are published by the to Domain Model.
    /// This "aggregate" maintains its own internal state that represents the state
    /// of a UI like a desktop application.  When this state changes, this ClientModel
    /// publishes high-level UI-specific events like: StuffAddedToInbox, ProjectAdded, etc.
    /// These UI events are not "event sourced" (persisted events which are then replayed to get state),
    /// they are in-memory "dumb" UI events that do not need the level of detail that 
    /// our persisted Event-Sourced Domain Model Events need. 
    /// Our Domain Events are still the same granular ones that matter for when we are
    /// persisting the Domain Model state changes and using them to resolve the 
    /// sync issues that come along with multi-user/device collaboration scenarios, but these
    /// in-memory "dumb" UI events do not need that level of detail to keep the UI up to date. 
    /// Basically, we are modeling the things that will eventually be represented
    /// on a client's screen.  Client Model is an underlying read model and notification sub-system 
    /// for implementing a client application.  Technically this Client Model context is reusable 
    /// across various client/device types (desktop, tablet, web, phone, etc.),
    /// but the practicality of reusing it remains to be seen.  
    /// </summary>
    public sealed class ClientModel
    {
        readonly IMessageQueue _queue;

        /// <summary> What's up with these "Immutable" things? 
        /// Notice the Immutable* things below.  The Immutability here means two things:
        /// 1) We could structure or model in such a way that it would be extremely "cheap"
        /// to publish events/copies that reference parts of this model.  These events can
        /// include as much information as needed.  They can be safely passed around because
        /// they are immutable and thus other code can't concurrently change them, so no
        /// nasty side effects that result from that.
        /// 2) It allows us to relax our mind when we think about this model because
        /// we can just publish the entire client state and know it contains whatever we
        /// may need to use from this model when the UI receives the event that contains it all.
        /// </summary>

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

        // LoadingCompleted is called by newly created instances of the ClientModel class
        // so that it can tell itself that all the Domain Event history has
        // been replayed and loaded into itself.  It now knows that it is
        // up to date with the state of the associated TrustedSystemId 
        public void LoadingCompleted()
        {
            _loadingCompleted = true;

            // take the TrustedSystem state that was loaded into this class and 
            // call my own methods to get a copy of it so I can put the current state
            // into an ImmutableClientModel read model that I can pass along to the UI,
            // in its entirety, as an argument to the Dumb.ClientModelLoaded UI event.
            var model = new ImmutableClientModel(
                GetInbox(),
                ImmutableList.Create(_projectList.Select(m => m.Freeze()).ToArray()));

            // give UI a way to get its hands on current state of the TrustedSystem
            // via a copy of the ENTIRE ImmutableClientModel
            // (i.e. The UI/world gets a copy of the the Inbox and ALL the Projects
            // and other relevant content it needs from the TrustedSystem do to its job.
            // The UI no longer needs to try to reach the ClientModel, to populate a view 
            // of the Inbox for example, because it is receiving an ENTIRE COPY 
            // of the ClientModel inside of the event that it subscribes to, and binds the UI to that!
            // In the event that comes to it, it has all the information it needs to display anything!
            // this means all the other UI components only need to subscribe to the same ClientModelLoaded
            // event to get the data they need to bind to. This same immutable data will be shared by
            // all consumers of the event so we wont waste memory just to give them each a "copy" to work with.
            // So this makes it really cheap and really reliable to have the UI use this, especially
            // if you have multiple processing threads.)
            // This is similar to how the Roslyn C# compiler handles its Abstract Syntaxt Tree.

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

            // the Client Model state has changed, so publish a "Dumb" UI event
            // to keep the contents of the UI current with the actual state of this model
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

            // we use "Freeze" to kind of fake immutability for now but makes it pretty
            // easy to replace with "real" immutability later
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