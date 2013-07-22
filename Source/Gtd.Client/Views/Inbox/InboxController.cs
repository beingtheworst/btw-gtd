using System.Collections.Generic;
using Gtd.Client.Models;

namespace Gtd.Client
{
    public class InboxController : 
        IHandle<AppInit>,
        IHandle<UI.DisplayInbox>,
        IHandle<InboxStuffCaptured>,  
        IHandle<InboxStuffArchived>, IHandle<FormLoaded>
    {
        readonly Region _dock;
        readonly IPublisher _queue;
        readonly ClientPerspective _perspective;

        readonly InboxView _control;

        InboxController(Region dock, IPublisher queue, ClientPerspective perspective)
        {
            _dock = dock;
            _queue = queue;
            _perspective = perspective;
            _control = new InboxView(this);
        }

        public static InboxController Wire(Region form, IPublisher queue, ISubscriber bus, ClientPerspective view)
        {
            var adapter = new InboxController(form, queue, view);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<UI.DisplayInbox>(adapter);
            bus.Subscribe<InboxStuffCaptured>(adapter);
            bus.Subscribe<InboxStuffArchived>(adapter);
            bus.Subscribe<FormLoaded>(adapter);

            return adapter;
        }

        public void Handle(AppInit message)
        {
            _dock.RegisterDock(_control, "inbox");
        }

        bool _shown; 

        public void Handle(UI.DisplayInbox message)
        {
            if (!_shown)
            {
                var inbox = _perspective.Model.GetInbox();
                _control.Sync(() => _control.LoadInbox(inbox));
                _shown = true;
            }

            _dock.SwitchTo("inbox");
            _queue.Publish(new UI.InboxDisplayed());
        }

        public void Handle(InboxStuffCaptured message)
        {
            _control.Sync(() => _control.AddThought(message.Thought, message.InboxStuffId));
            
        }
        public void Handle(InboxStuffArchived message)
        {
            _control.Sync(() => _control.RemoveThought(message.InboxStuffId));
        }

        public void WhenRequestedThoughtsArchival(IEnumerable<InboxStuffId> thoughtIds)
        {
            foreach (var id in thoughtIds)
            {
                _queue.Publish(new UI.ArchiveThoughtClicked(id));
            }
        }

        public void WhenRequestedMoveThoughtsToProject(ProjectId id, InboxStuffId[] inboxStuffIds)
        {
            _queue.Publish(new UI.MoveThoughtsToProjectClicked(inboxStuffIds, id));
        }

        public void WhenCaptureThoughtClicked()
        {
            _queue.Publish(new UI.CaptureThoughtClicked());
        }

        public IList<ImmutableProject> ListProjects()
        {
            return _perspective.ListProjects();
        }

        public void Handle(FormLoaded message)
        {
            
        }
    }
}