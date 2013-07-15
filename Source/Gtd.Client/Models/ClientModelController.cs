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
        IHandle<Ui.FilterChanged>
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
            bus.Subscribe<Ui.FilterChanged>(this);
        }


        public void Handle(TrustedSystemCreated e)
        {
            _current.Verify(e.Id);
            _current.Create(e.Id);
        }

        public void Handle(ThoughtCaptured e)
        {
            _current.Verify(e.Id);
            _current.ThoughtCaptured(e.ThoughtId, e.Thought, e.TimeUtc);
        }
        public void Handle(ThoughtArchived e)
        {
            _current.Verify(e.Id);
            _current.ThoughtArchived(e.ThoughtId);
        }

        public void Handle(ProjectDefined e)
        {
            _current.Verify(e.Id);
            _current.ProjectDefined(e.ProjectId, e.ProjectOutcome, e.Type);
        }
        public void Handle(ActionDefined e)
        {
            _current.Verify(e.Id);
            _current.ActionDefined(e.ProjectId, e.ActionId, e.Outcome);
        }
        public void Handle(ActionCompleted e)
        {
            _current.Verify(e.Id);
            _current.ActionCompleted(e.ActionId);
        }
        public void Handle(ActionOutcomeChanged e)
        {
            _current.Verify(e.Id);
            _current.ActionOutcomeChanged(e.ActionId, e.ActionOutcome);
        }

        public void Handle(ProjectOutcomeChanged e)
        {
            _current.Verify(e.Id);
            _current.ProjectOutcomeChanged(e.ProjectId, e.ProjectOutcome);
        }

        public void Handle(ThoughtSubjectChanged e)
        {
            _current.Verify(e.Id);
            _current.ThoughtSubjectChanged(e.ThoughtId, e.Subject);
        }
        public void Handle(ActionArchived e)
        {
            _current.Verify(e.Id);
            _current.ActionArchived(e.ActionId);
        }
        public void Handle(ProjectTypeChanged e)
        {
            _current.Verify(e.Id);
            _current.ProjectTypeChanged(e.ProjectId, e.Type);
        }
        public void Handle(StartDateAssignedToAction e)
        {
            _current.Verify(e.Id);
            _current.StartDateAssigned(e.ActionId, e.NewStartDate);
        }
        public void Handle(DueDateAssignedToAction e)
        {
            _current.Verify(e.Id);
            _current.DueDateAssigned(e.ActionId, e.NewDueDate);
        }
        public void Handle(StartDateRemovedFromAction e)
        {
            _current.Verify(e.Id);
            _current.StartDateAssigned(e.ActionId, DateTime.MinValue);
        }
        public void Handle(DueDateRemovedFromAction e)
        {
            _current.Verify(e.Id);
            _current.DueDateAssigned(e.ActionId, DateTime.MinValue);
        }
        public void Handle(ActionStartDateMoved e)
        {
            _current.Verify(e.Id);
            _current.StartDateAssigned(e.ActionId, e.NewStartDate);
        }
        public void Handle(ActionDueDateMoved e)
        {
            _current.Verify(e.Id);
            _current.DueDateAssigned(e.ActionId, e.NewDueDate);
        }

        ClientModel _current;

        public void Handle(ProfileLoaded evt)
        {
            if (_current != null && _current.Id == evt.SystemId)
            {
                return;
            }

            _current = new ClientModel();
            
            foreach (var e in _store.LoadEventStream(evt.SystemId.ToStreamId()).Events)
            {
                ((dynamic) this).Handle((dynamic) e);
            }
            _provider.SwitchToModel(_current);

            _queue.Enqueue(new ClientModelLoaded());
        }


        public void Handle(Ui.FilterChanged message)
        {
            _provider.SwitchToFilter(message.Criteria);
        }
    }
}