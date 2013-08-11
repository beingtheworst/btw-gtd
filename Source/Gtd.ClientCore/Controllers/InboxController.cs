using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Gtd.Client.Models;
using System.Linq;
using Gtd.ClientCore;

namespace Gtd.Client
{

    public interface IInboxView
    {
        void SubscribeToTrashStuffClick(Action<IImmutableList<StuffId>> callback);
        void SubscribeToAddStuffClick(Action callback);
        void SubscribeToListProjects(Func<ICollection<ImmutableProjectInfo>> request);
        void SubscribeToMoveStuffToProject(Action<ProjectId, IImmutableList<StuffId>> callback);
        
        void SubscribeToStartDrag(Action<DragSubject<StuffId>> callback);

        void ShowInbox(ImmutableInbox inbox);
        void AddStuff(ImmutableStuff stuff);
        void RemoveStuff(StuffId stuff);
    }

    public class InboxController : 
        
        IHandle<UI.DisplayInbox>,
        IHandle<Dumb.StuffAddedToInbox>,  
        IHandle<Dumb.StuffRemovedFromInbox>
    {
        
        readonly IMessageQueue _queue;
        readonly ClientPerspective _perspective;

        readonly IInboxView _control;

        InboxController(IInboxView view, IMessageQueue queue, ClientPerspective perspective)
        {
            
            _queue = queue;
            _perspective = perspective;
            _control = view;
        }

        public static InboxController Wire(IInboxView view, IMessageQueue queue, ISubscriber bus, ClientPerspective model)
        {
            var adapter = new InboxController(view, queue, model);

            
            bus.Subscribe<UI.DisplayInbox>(adapter);
            bus.Subscribe<Dumb.StuffAddedToInbox>(adapter);
            bus.Subscribe<Dumb.StuffRemovedFromInbox>(adapter);

            view.SubscribeToTrashStuffClick(adapter.TrashStuff);
            view.SubscribeToAddStuffClick(adapter.AddStuff);
            view.SubscribeToListProjects(adapter.ListProjects);
            view.SubscribeToMoveStuffToProject(adapter.MoveStuffToProject);
            view.SubscribeToStartDrag(adapter.StartDrag);

            return adapter;
        }

        void StartDrag(DragSubject<StuffId> subject)
        {
            _queue.Enqueue(new UI.DragStarted(new StuffDragManager(subject.Request, ImmutableList.Create(subject.Subject), _queue)));
        }


        public void Handle(UI.DisplayInbox message)
        {
            var inbox = _perspective.Model.GetInbox();

            _control.ShowInbox(inbox);


            _queue.Enqueue(new UI.InboxDisplayed());
        }

        public void Handle(Dumb.StuffAddedToInbox message)
        {
            _control.AddStuff(message.Stuff);
            
        }
        public void Handle(Dumb.StuffRemovedFromInbox message)
        {
            _control.RemoveStuff(message.Stuff.StuffId);
        }

        public void TrashStuff(IEnumerable<StuffId> stuffIds)
        {
            foreach (var id in stuffIds)
            {
                _queue.Enqueue(new UI.TrashStuffClicked(id));
            }
        }

        public void MoveStuffToProject(ProjectId projectId, IImmutableList<StuffId> stuffIdsToMove)
        {
            _queue.Enqueue(new UI.MoveStuffToProjectClicked(stuffIdsToMove, projectId));
        }

        public void AddStuff()
        {
            _queue.Enqueue(new UI.AddStuffClicked());
        }

        public ICollection<ImmutableProjectInfo> ListProjects()
        {
            return _perspective.ListProjects().Select(i => i.Info).ToList();
        }
    }
}