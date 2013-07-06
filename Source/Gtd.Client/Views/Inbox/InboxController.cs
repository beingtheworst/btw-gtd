using System.Collections.Generic;

namespace Gtd.Client
{
    public class InboxController : 
        IHandle<AppInit>,
        IHandle<Ui.DisplayInbox>,
        IHandle<ThoughtCaptured>,  
        IHandle<ThoughtArchived>, IHandle<FormLoaded>
    {
        readonly Region _dock;
        readonly IPublisher _queue;
        readonly ISystemView _view;

        readonly InboxView _control;

        InboxController(Region dock, IPublisher queue, ISystemView view)
        {
            _dock = dock;
            _queue = queue;
            _view = view;
            _control = new InboxView(this);
        }

        public static InboxController Wire(Region form, IPublisher queue, ISubscriber bus, ISystemView view)
        {
            var adapter = new InboxController(form, queue, view);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<Ui.DisplayInbox>(adapter);
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

        public void Handle(Ui.DisplayInbox message)
        {
            if (!_shown)
            {
                _control.Sync(() => _control.LoadThoughts(_view.ListInbox()));
                _shown = true;
            }

            _dock.SwitchTo("inbox");
            _queue.Publish(new Ui.InboxDisplayed());
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
                _queue.Publish(new Ui.ArchiveThought(id));
            }
        }

        public void WhenRequestedMoveThoughtsToProject(ProjectId id, ThoughtId[] thoughtIds)
        {
            _queue.Publish(new Ui.MoveThoughtsToProject(thoughtIds, id));
        }

        public void WhenCaptureThoughtClicked()
        {
            _queue.Publish(new Ui.CaptureThoughtClicked());
        }

        public IList<ProjectView> ListProjects()
        {
            return _view.ListProjects();
        }

        public void Handle(FormLoaded message)
        {
            
        }
    }
}