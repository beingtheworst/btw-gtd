using System;
using Gtd.CoreDomain;
using Microsoft.CSharp.RuntimeBinder;

namespace Gtd.Client.Models
{
    public sealed class ClientModelController :
        IHandle<TrustedSystemCreated>,
        IHandle<StuffPutInInbox>,
        IHandle<StuffTrashed>,
        IHandle<StuffArchived>,
        IHandle<ProjectDefined>,
        IHandle<ActionDefined>,
        IHandle<ActionCompleted>,
        IHandle<ProfileLoaded>, 
        IHandle<ActionOutcomeChanged>,
        IHandle<UI.FilterChanged>
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
            bus.Subscribe<UI.FilterChanged>(this);
        }

        

        bool CanHandle(TrustedSystemId system)
        {
            if (_model == null)
                return false;
            if (system.Id != _model.Id.Id)
                return false;
            return true;
        }


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

        public void Handle(ProfileLoaded evt)
        {
            if (_model != null && _model.Id == evt.SystemId)
            {
                // we already have this model all loaded up
                return;
            }

            // ok, create new model
            _model = new ClientModel(_queue, evt.SystemId);
            
            // replay all events, since we don't have a snapshot for now
            var stream = _eventStore.LoadEventStream(evt.SystemId.ToStreamId());
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

        public void Handle(UI.FilterChanged message)
        {
            _provider.SwitchToFilter(message.Criteria);
        }

        public void Handle(StuffArchived e)
        {
            if (CanHandle(e.Id))
                _model.StuffArchived(e.StuffId);
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
            public readonly ActionId ActionId;
            public readonly string ActionUIKey;

            public readonly ProjectId ProjectId;
            public readonly string ProjectUIKey;
            public readonly string Outcome;

            public ActionAdded(ActionId actionId, string actionUIKey, ProjectId projectId, string projectUIKey, string outcome)
            {
                ActionId = actionId;
                ActionUIKey = actionUIKey;
                ProjectId = projectId;
                ProjectUIKey = projectUIKey;
                Outcome = outcome;
            }
        }

        public sealed class ActionUpdated : CliendModelEvent
        {

            public readonly ProjectId ProjectId;
            public readonly string ProjectUIKey;
            public readonly ImmutableAction Action;
            public ActionUpdated(
                ImmutableAction action,
                ProjectId projectId, string projectUIKey)
            {

                Action = action;
                ProjectId = projectId;
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