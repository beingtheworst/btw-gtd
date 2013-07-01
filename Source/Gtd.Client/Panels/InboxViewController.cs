using System.Collections.Generic;

namespace Gtd.Client
{
    public class InboxViewController : 
        IHandle<AppInit>,
        IHandle<RequestShowInbox>,
        IHandle<ThoughtCaptured>,  
        IHandle<ThoughtArchived>

    {
        readonly IMainDock _dock;
        readonly IPublisher _queue;

        readonly InboxView _control;

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<RequestShowInbox>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
        }

        public InboxViewController(IMainDock dock, IPublisher queue, ISystemView view)
        {
            _dock = dock;
            _queue = queue;
            _control = new InboxView(queue, view);
            
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
            _control.AddThought(message.Thought, message.ThoughtId);
        }
        public void Handle(ThoughtArchived message)
        {
            _control.RemoveThought(message.ThoughtId);
        }
    }
}