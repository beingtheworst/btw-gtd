using System.Collections.Generic;

namespace Gtd.Client
{
    public class InboxViewController : 
        IHandle<AppInit>, IHandle<RequestShowInbox>, IHandle<ThoughtCaptured>,  IHandle<ThoughtArchived>, IHandle<ProjectDefined>

    {
        readonly IMainDock _dock;
        readonly IPublisher _queue;

        readonly InboxView _view;

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<RequestShowInbox>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
            bus.Subscribe<ProjectDefined>(this);
        }

        public InboxViewController(IMainDock dock, IPublisher queue, ISystemView view)
        {
            _dock = dock;
            _queue = queue;
            _view = new InboxView(queue, view);
            
        }

        public void Handle(AppInit message)
        {
            _dock.RegisterDock(_view, "inbox");
        }

        public void Handle(RequestShowInbox message)
        {
            _dock.SwitchTo("inbox");
            _queue.Publish(new InboxShown());

        }

        public void Handle(ThoughtCaptured message)
        {
            _view.AddThought(message.Thought, message.ThoughtId);
        }
        public void Handle(ThoughtArchived message)
        {
            _view.RemoveThought(message.ThoughtId);
        }

        IDictionary<ProjectId,string> _projects = new Dictionary<ProjectId, string>(); 


        public void Handle(ProjectDefined message)
        {
            _projects.Add(message.ProjectId, message.ProjectOutcome);
        }
    }
}