using System.Collections.Generic;
using Gtd.Client.Models;

namespace Gtd.Client
{
    public class InboxController : 
        IHandle<AppInit>,
        IHandle<UI.DisplayInbox>,
        IHandle<ThoughtCaptured>,  
        IHandle<ThoughtArchived>, IHandle<FormLoaded>
    {
        readonly Region _dock;
        readonly IPublisher _queue;
        readonly ClientPerspective _view;

        readonly InboxView _control;

        InboxController(Region dock, IPublisher queue, ClientPerspective view)
        {
            _dock = dock;
            _queue = queue;
            _view = view;
            _control = new InboxView(this);
        }

        public static InboxController Wire(Region form, IPublisher queue, ISubscriber bus, ClientPerspective view)
        {
            var adapter = new InboxController(form, queue, view);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<UI.DisplayInbox>(adapter);
            bus.Subscribe<ThoughtCaptured>(adapter);
            bus.Subscribe<ThoughtArchived>(adapter);
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
                _control.Sync(() => _control.LoadThoughts(_view.ListInbox()));
                _shown = true;
            }

            _dock.SwitchTo("inbox");
            _queue.Publish(new UI.InboxDisplayed());
        }

        public void Handle(ThoughtCaptured message)
        {
            _control.Sync(() => _control.AddThought(message.Thought, message.ThoughtId));
            
        }
        public void Handle(ThoughtArchived message)
        {
            _control.Sync(() => _control.RemoveThought(message.ThoughtId));
        }

        public void WhenRequestedThoughtsArchival(IEnumerable<ThoughtId> thoughtIds)
        {
            foreach (var id in thoughtIds)
            {
                _queue.Publish(new UI.ArchiveThoughtClicked(id));
            }
        }

        public void WhenRequestedMoveThoughtsToProject(ProjectId id, ThoughtId[] thoughtIds)
        {
            _queue.Publish(new UI.MoveThoughtsToProjectClicked(thoughtIds, id));
        }

        public void WhenCaptureThoughtClicked()
        {
            _queue.Publish(new UI.CaptureThoughtClicked());
        }

        public IList<ProjectModel> ListProjects()
        {
            return _view.ListProjects();
        }

        public void Handle(FormLoaded message)
        {
            
        }
    }
}