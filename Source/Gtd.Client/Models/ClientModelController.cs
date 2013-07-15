using System;
using Gtd.CoreDomain;

namespace Gtd.Client.Models
{
    public sealed class ClientModelController :
        IHandle<TrustedSystemCreated>,
        IHandle<ThoughtCaptured>,
        IHandle<ThoughtArchived>,
        IHandle<ProjectDefined>,
        IHandle<ActionDefined>,
        IHandle<ActionCompleted>,
        IHandle<ProfileLoaded>, 
        IHandle<UI.FilterChanged>
    {
        readonly IEventStore _store;
        readonly ClientPerspective _provider;
        readonly IMessageQueue _queue;

        ClientModelController(IEventStore store, ClientPerspective provider, IMessageQueue queue)
        {
            _store = store;
            _provider = provider;
            _queue = queue;
        }

        public static void WireTo(IEventStore store, ClientPerspective provider, ISubscriber subscriber, IMessageQueue queue)
        {
            var controller = new ClientModelController(store, provider, queue);
            controller.SubscribeTo(subscriber);
        }

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<TrustedSystemCreated>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
            bus.Subscribe<ProjectDefined>(this);
            bus.Subscribe<ActionDefined>(this);
            bus.Subscribe<ActionCompleted>(this);
            bus.Subscribe<ProfileLoaded>(this);
            bus.Subscribe<UI.FilterChanged>(this);
        }


        public void Handle(TrustedSystemCreated e)
        {
            _model.Verify(e.Id);
            _model.Create(e.Id);
        }

        public void Handle(ThoughtCaptured e)
        {
            _model.Verify(e.Id);
            _model.ThoughtCaptured(e.ThoughtId, e.Thought, e.TimeUtc);
        }
        public void Handle(ThoughtArchived e)
        {
            _model.Verify(e.Id);
            _model.ThoughtArchived(e.ThoughtId);
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

        public void Handle(ThoughtSubjectChanged e)
        {
            _model.Verify(e.Id);
            _model.ThoughtSubjectChanged(e.ThoughtId, e.Subject);
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
        public void Handle(StartDateAssignedToAction e)
        {
            _model.Verify(e.Id);
            _model.StartDateAssigned(e.ActionId, e.NewStartDate);
        }
        public void Handle(DueDateAssignedToAction e)
        {
            _model.Verify(e.Id);
            _model.DueDateAssigned(e.ActionId, e.NewDueDate);
        }
        public void Handle(StartDateRemovedFromAction e)
        {
            _model.Verify(e.Id);
            _model.StartDateAssigned(e.ActionId, DateTime.MinValue);
        }
        public void Handle(DueDateRemovedFromAction e)
        {
            _model.Verify(e.Id);
            _model.DueDateAssigned(e.ActionId, DateTime.MinValue);
        }
        public void Handle(ActionStartDateMoved e)
        {
            _model.Verify(e.Id);
            _model.StartDateAssigned(e.ActionId, e.NewStartDate);
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
            foreach (var e in _store.LoadEventStream(evt.SystemId.ToStreamId()).Events)
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

    public static class Cm
    {
        public abstract class CliendModelEvent : Message
        {
             
        }

        public sealed class ClientModelLoaded : CliendModelEvent
        {

        }

        public sealed class ProjectDefined : CliendModelEvent
        {
            public readonly string UniqueKey;
            public readonly string ProjectOutcome;
            public readonly ProjectId ProjectId;

            public ProjectDefined(string uniqueKey, string projectOutcome, ProjectId projectId)
            {
                UniqueKey = uniqueKey;
                ProjectOutcome = projectOutcome;
                ProjectId = projectId;
            }
        }
        
    }

}