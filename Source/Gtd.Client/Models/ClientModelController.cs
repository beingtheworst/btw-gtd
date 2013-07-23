using System;
using System.Collections.Immutable;
using Gtd.CoreDomain;

namespace Gtd.Client.Models
{
    public sealed class ClientModelController :
        IHandle<TrustedSystemCreated>,
        IHandle<StuffPutInInbox>,
        IHandle<StuffTrashed>,
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
            bus.Subscribe<ProjectDefined>(this);
            bus.Subscribe<ActionDefined>(this);
            bus.Subscribe<ActionCompleted>(this);
            bus.Subscribe<ProfileLoaded>(this);
            bus.Subscribe<ActionOutcomeChanged>(this);
            bus.Subscribe<UI.FilterChanged>(this);
        }


        public void Handle(TrustedSystemCreated e)
        {
            _model.Verify(e.Id);
            _model.Create(e.Id);
        }

        public void Handle(StuffPutInInbox e)
        {
            _model.Verify(e.Id);
            _model.StuffPutInInbox(e.StuffId, e.StuffDescription, e.TimeUtc);
        }
        public void Handle(StuffTrashed e)
        {
            _model.Verify(e.Id);
            _model.StuffTrashed(e.StuffId);
        }

        public void Handle(ProjectDefined e)
        {
            _model.Verify(e.Id);
            _model.ProjectDefined(e.ProjectId, e.ProjectOutcome, e.Type);
        }
        public void Handle(ActionDefined e)
        {
            _model.Verify(e.Id);
            _model.ActionDefined(e.ProjectId, e.ActionId, e.Outcome);
        }
        public void Handle(ActionCompleted e)
        {
            _model.Verify(e.Id);
            _model.ActionCompleted(e.ActionId);
        }
        public void Handle(ActionOutcomeChanged e)
        {
            _model.Verify(e.Id);
            _model.ActionOutcomeChanged(e.ActionId, e.ActionOutcome);
        }

        public void Handle(ProjectOutcomeChanged e)
        {
            _model.Verify(e.Id);
            _model.ProjectOutcomeChanged(e.ProjectId, e.ProjectOutcome);
        }

        public void Handle(StuffDescriptionChanged e)
        {
            _model.Verify(e.Id);
            _model.DescriptionOfStuffChanged(e.StuffId, e.NewDescriptionOfStuff);
        }
        public void Handle(ActionArchived e)
        {
            _model.Verify(e.Id);
            _model.ActionArchived(e.ActionId);
        }
        public void Handle(ProjectTypeChanged e)
        {
            _model.Verify(e.Id);
            _model.ProjectTypeChanged(e.ProjectId, e.Type);
        }
        public void Handle(ActionDeferredUntil e)
        {
            _model.Verify(e.Id);
            _model.DeferredUtil(e.ActionId, e.DeferUntil);
        }
        public void Handle(DueDateAssignedToAction e)
        {
            _model.Verify(e.Id);
            _model.DueDateAssigned(e.ActionId, e.NewDueDate);
        }
        public void Handle(ActionIsNoLongerDeferred e)
        {
            _model.Verify(e.Id);
            _model.DeferredUtil(e.ActionId, DateTime.MinValue);
        }
        public void Handle(DueDateRemovedFromAction e)
        {
            _model.Verify(e.Id);
            _model.DueDateAssigned(e.ActionId, DateTime.MinValue);
        }
        public void Handle(ActionDeferDateShifted e)
        {
            _model.Verify(e.Id);
            _model.DeferredUtil(e.ActionId, e.NewDeferDate);
        }
        public void Handle(ActionDueDateMoved e)
        {
            _model.Verify(e.Id);
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
            foreach (var e in _eventStore.LoadEventStream(evt.SystemId.ToStreamId()).Events)
            {
                ((dynamic) this).Handle((dynamic) e);
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
            public readonly ActionId ActionId;
            public readonly string ActionUIKey;

            public readonly ProjectId ProjectId;
            public readonly string ProjectUIKey;
            public readonly string Outcome;
            public readonly bool Completed;

            public ActionUpdated(ActionId actionId, string actionUIKey, ProjectId projectId, string projectUIKey, string outcome, bool completed)
            {
                ActionId = actionId;
                ActionUIKey = actionUIKey;
                ProjectId = projectId;
                ProjectUIKey = projectUIKey;
                Outcome = outcome;
                Completed = completed;
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