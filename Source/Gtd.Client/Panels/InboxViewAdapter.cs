using System.Collections.Generic;

namespace Gtd.Client
{
    public class InboxViewAdapter : 
        IHandle<AppInit>,
        IHandle<RequestShowInbox>,
        IHandle<ThoughtCaptured>,  
        IHandle<ThoughtArchived>
    {
        readonly IMainDock _dock;
        readonly IPublisher _queue;
        readonly ISystemView _view;

        readonly InboxView _control;

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<RequestShowInbox>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
        }

        public InboxViewAdapter(IMainDock dock, IPublisher queue, ISystemView view)
        {
            _dock = dock;
            _queue = queue;
            _view = view;
            _control = new InboxView(this);
        }

        public void Handle(AppInit message)
        {
            _dock.RegisterDock(_control, "inbox");
        }

        public void Handle(RequestShowInbox message)
        {
            _dock.SwitchTo("inbox");
            _queue.Publish(new InboxShown());
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
                _queue.Publish(new RequestArchiveThought(id));
            }
        }

        public void WhenRequestedMoveThoughtsToProject(ProjectId id, ThoughtId[] thoughtIds)
        {
            _queue.Publish(new RequestMoveThoughtsToProject(thoughtIds, id));
        }

        public void WhenCaptureThoughtClicked()
        {
            _queue.Publish(new CaptureThoughtClicked());
        }

        public IList<ProjectView> ListProjects()
        {
            return _view.ListProjects();
        }
    }
}