using System.Collections.Generic;
using Gtd.Client.Models;

namespace Gtd.Client
{
    public class InboxController : 
        IHandle<AppInit>,
        IHandle<UI.DisplayInbox>,
        IHandle<StuffPutInInbox>,  
        IHandle<StuffTrashed>, IHandle<FormLoaded>
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
            bus.Subscribe<StuffPutInInbox>(adapter);
            bus.Subscribe<StuffTrashed>(adapter);
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

        public void Handle(StuffPutInInbox message)
        {
            _control.Sync(() => _control.AddStuff(message.StuffDescription, message.StuffId));
            
        }
        public void Handle(StuffTrashed message)
        {
            _control.Sync(() => _control.TrashStuff(message.StuffId));
        }

        public void WhenRequestedToTrashStuff(IEnumerable<StuffId> stuffIds)
        {
            foreach (var id in stuffIds)
            {
                _queue.Publish(new UI.TrashStuffClicked(id));
            }
        }

        public void WhenRequestedToMoveStuffToProject(ProjectId projectId, StuffId[] stuffIdsToMove)
        {
            _queue.Publish(new UI.MoveStuffToProjectClicked(stuffIdsToMove, projectId));
        }

        public void WhenAddStuffClicked()
        {
            _queue.Publish(new UI.AddStuffClicked());
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