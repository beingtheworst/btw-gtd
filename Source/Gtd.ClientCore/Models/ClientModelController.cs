using System;
using Gtd.ClientCore;
using Gtd.CoreDomain;
using Microsoft.CSharp.RuntimeBinder;

namespace Gtd.Client.Models
{
    /// <summary>
    /// This ClientModelController sits between the Messaging sub-system
    /// of this client application, and different components that handle the messages.
    /// Client Controllers basically compartmentalize a messaging unit of functionality
    /// (they are responsible for managing functionality).  Controllers are not
    /// linked directly to UI-specific code (like WinForms, WPF, etc.) so they are
    /// theoretically reusable across various UI-implementation technologies.
    /// (That is theoretically true for the entire Gtd.ClientCore project.)
    /// </summary>
    public sealed class ClientModelController :
        IHandle<TrustedSystemCreated>,
        IHandle<StuffPutInInbox>,
        IHandle<StuffTrashed>,
        IHandle<StuffArchived>,
        IHandle<ProjectDefined>,
        IHandle<ActionDefined>,
        IHandle<ActionCompleted>,
        IHandle<ProfileLoaded>, 
        IHandle<ActionMovedToProject>,
        IHandle<ActionOutcomeChanged>,
        IHandle<UI.ActionFilterChanged>
    {
        readonly IEventStore _eventStore;
        readonly ClientPerspective _provider;
        readonly IMessageQueue _queue;

        ClientModelController(IEventStore eventStore, ClientPerspective provider, IMessageQueue queue)
        {
            _eventStore = eventStore;
            _provider = provider;
            _queue = queue;
        }

        public static void WireTo(IEventStore eventStore, ClientPerspective provider, ISubscriber subscriber, IMessageQueue queue)
        {
            var controller = new ClientModelController(eventStore, provider, queue);
            controller.SubscribeTo(subscriber);
        }

        // subscribe to the events on the in-memory message bus of the system
        // so that this class can wire the events below to the Client Model
        // so that the Client Model can react/handle those events and change its state
        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<TrustedSystemCreated>(this);
            bus.Subscribe<StuffPutInInbox>(this);
            bus.Subscribe<StuffTrashed>(this);
            bus.Subscribe<StuffArchived>(this);
            bus.Subscribe<ProjectDefined>(this);
            bus.Subscribe<ActionDefined>(this);
            bus.Subscribe<ActionCompleted>(this);
            bus.Subscribe<ProfileLoaded>(this);
            bus.Subscribe<ActionOutcomeChanged>(this);
            bus.Subscribe<ActionMovedToProject>(this);


            bus.Subscribe<UI.ActionFilterChanged>(this);
        }

        

        bool CanHandle(TrustedSystemId system)
        {
            if (_model == null)
                return false;
            if (system.Id != _model.Id.Id)
                return false;
            return true;
        }

        // When this Client Application starts via the AppController, 
        // it picks the TrustedSystem that is associated with the current
        // installation.  If one does not exist, it creates one.
        public void Handle(TrustedSystemCreated e)
        {
            if (CanHandle(e.Id))
            _model.Create(e.Id);
        }

        public void Handle(StuffPutInInbox e)
        {
            if (CanHandle(e.Id))
            _model.StuffPutInInbox(e.StuffId, e.StuffDescription, e.TimeUtc);
        }
        public void Handle(StuffTrashed e)
        {
            if (CanHandle(e.Id))
            _model.StuffTrashed(e.StuffId);
        }

        public void Handle(ProjectDefined e)
        {
            if (CanHandle(e.Id))
            _model.ProjectDefined(e.ProjectId, e.ProjectOutcome, e.Type);
        }
        public void Handle(ActionDefined e)
        {
            if (CanHandle(e.Id))
            _model.ActionDefined(e.ProjectId, e.ActionId, e.Outcome);
        }
        public void Handle(ActionCompleted e)
        {
            if (CanHandle(e.Id))
            _model.ActionCompleted(e.ActionId);
        }
        public void Handle(ActionOutcomeChanged e)
        {
            if (CanHandle(e.Id))
            _model.ActionOutcomeChanged(e.ActionId, e.ActionOutcome);
        }

        public void Handle(ProjectOutcomeChanged e)
        {
            if (CanHandle(e.Id))
            _model.ProjectOutcomeChanged(e.ProjectId, e.ProjectOutcome);
        }

        public void Handle(StuffDescriptionChanged e)
        {
            if (CanHandle(e.Id))
            _model.DescriptionOfStuffChanged(e.StuffId, e.NewDescriptionOfStuff);
        }
        public void Handle(ActionArchived e)
        {
            if (CanHandle(e.Id))
            _model.ActionArchived(e.ActionId);
        }
        public void Handle(ProjectTypeChanged e)
        {
            if (CanHandle(e.Id))
            _model.ProjectTypeChanged(e.ProjectId, e.Type);
        }
        public void Handle(ActionDeferredUntil e)
        {
            if (CanHandle(e.Id))
            _model.DeferredUtil(e.ActionId, e.DeferUntil);
        }
        public void Handle(DueDateAssignedToAction e)
        {
            if (CanHandle(e.Id))
            _model.DueDateAssigned(e.ActionId, e.NewDueDate);
        }
        public void Handle(ActionIsNoLongerDeferred e)
        {
            if (CanHandle(e.Id))
            _model.DeferredUtil(e.ActionId, DateTime.MinValue);
        }
        public void Handle(DueDateRemovedFromAction e)
        {
            if (CanHandle(e.Id))
            _model.DueDateAssigned(e.ActionId, DateTime.MinValue);
        }
        public void Handle(ActionDeferDateShifted e)
        {
            if (CanHandle(e.Id))
            _model.DeferredUtil(e.ActionId, e.NewDeferDate);
        }
        public void Handle(ActionDueDateMoved e)
        {
            if (CanHandle(e.Id))
            _model.DueDateAssigned(e.ActionId, e.NewDueDate);
        }

        ClientModel _model;


        // handle the ProfileLoaded event that the AppController
        // will publish from its InitApplication method when the App starts up
        public void Handle(ProfileLoaded evt)
        {
            if (_model != null && _model.Id == evt.TrustedSystemId)
            {
                // we already have this client model with the event history
                // for the correct TrustedSystemId all loaded up so just:
                return;
            }

            // ok, need to create a new client model
            // and load the event history for the selected TrustedSystemId 
            _model = new ClientModel(_queue, evt.TrustedSystemId);
            
            // replay all events for the relevant TrustedSystemId and load
            // it into this Client Model to represent its current state,
            // (since we don't have a snapshot for now)
            var stream = _eventStore.LoadEventStream(evt.TrustedSystemId.ToStreamId());
            foreach (var e in stream.Events)
            {
                try
                {
                    ((dynamic) this).Handle((dynamic) e);
                }
                catch (RuntimeBinderException ex)
                {
                    throw new InvalidOperationException("Failed to exec " + e.GetType().Name, ex);
                }
            }
            // now enable sending new events when we update

            _model.LoadingCompleted();

            // switch our provider to this new model
            _provider.SwitchToModel(_model);
        }

        public void Handle(UI.ActionFilterChanged message)
        {
            _provider.SwitchToFilter(message.Criteria);
        }

        public void Handle(StuffArchived e)
        {
            if (CanHandle(e.Id))
                _model.StuffArchived(e.StuffId);
        }

        public void Handle(ActionMovedToProject e)
        {
            if (CanHandle(e.Id))
                _model.ActionMoved(e.ActionId, e.OldProject, e.NewProject);
        }
    }

    public static class Dumb
    {
        public abstract class CliendModelEvent : Message
        {
             
        }

        public sealed class ClientModelLoaded : CliendModelEvent
        {
            public readonly ImmutableClientModel Model;

            public ClientModelLoaded(ImmutableClientModel model)
            {
                Model = model;
            }
        }

        public sealed class StuffAddedToInbox : CliendModelEvent
        {
            public readonly ImmutableStuff Stuff;
            public readonly int InboxCount;
            public StuffAddedToInbox(ImmutableStuff stuffThatWasAdded, int inboxCount)
            {
                Stuff = stuffThatWasAdded;
                InboxCount = inboxCount;
            }
        }

        public sealed class StuffUpdated : CliendModelEvent
        {
            public readonly ImmutableStuff Stuff;

            public StuffUpdated(ImmutableStuff stuff)
            {
                Stuff = stuff;
            }
        }

        public sealed class StuffRemovedFromInbox : CliendModelEvent
        {
            public readonly ImmutableStuff Stuff;
            public readonly int InboxCount;
            public StuffRemovedFromInbox(ImmutableStuff stuffThatWasRemoved, int inboxCount)
            {
                Stuff = stuffThatWasRemoved;
                InboxCount = inboxCount;
            }
        }

        public sealed class ActionAdded : CliendModelEvent
        {
            public readonly ImmutableAction Action;

            public ActionAdded(ImmutableAction action)
            {
                Action = action;
            }
        }

        public sealed class ActionRemoved : CliendModelEvent
        {
            public readonly ImmutableAction Action;
            public ActionRemoved(ImmutableAction action)
            {
                Action = action;
            }
        }

        public sealed class ActionUpdated : CliendModelEvent
        {

            
            public readonly string ProjectUIKey;
            public readonly ImmutableAction Action;
            public ActionUpdated(
                ImmutableAction action,string projectUIKey)
            {

                Action = action;
                ProjectUIKey = projectUIKey;
            }
        }

        public sealed class ProjectAdded : CliendModelEvent
        {
            public readonly string UniqueKey;
            public readonly string ProjectOutcome;
            public readonly ProjectId ProjectId;

            public ProjectAdded(string uniqueKey, string projectOutcome, ProjectId projectId)
            {
                UniqueKey = uniqueKey;
                ProjectOutcome = projectOutcome;
                ProjectId = projectId;
            }
        }
        
    }

}