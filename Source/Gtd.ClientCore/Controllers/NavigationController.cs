using System;
using System.Collections.Generic;
using Gtd.Client.Models;
using System.Linq;

namespace Gtd.Client.Views.Navigation
{

    public interface INavigationView
    {
        void UpdateInboxNode(int count);
        void ReloadProjectList(IList<ImmutableProjectInfo> projects);
        

        void WhenInboxSelected(Action action);
        void WhenProjectSelected(Action<string> project);
        void SubscribeDragOver(DragController controller);
        //void SubscribeDragOver();
        void SelectProject(string uiKey);
        void SelectInbox();
    }

    public sealed class DragController
    {

        string _request;
        object _subject;

        IMessageQueue _queue;
        public DragController(IMessageQueue queue)
        {
            _queue = queue;
        }

        public void WhenDraggingStuff(string request, StuffId id)
        {
            _subject = id;
            
            _request = request;
        }
        
        public bool CanAcceptDragOverProject(string request)
        {
            if (request == null) throw new ArgumentNullException("request");
            if (request != _request) throw new InvalidOperationException();

            return _subject is StuffId;

        }

        public void DropToProject(string request, ProjectId id)
        {
            if (!CanAcceptDragOverProject(request)) throw new InvalidOperationException();
            _queue.Enqueue(new UI.MoveStuffToProjectClicked(new StuffId[]{(StuffId) _subject}, id));
        }
    }

    public sealed class NavigationController : 
        IHandle<Dumb.ClientModelLoaded>,
        IHandle<Dumb.StuffAddedToInbox>, 
        IHandle<Dumb.StuffRemovedFromInbox>,
        IHandle<Dumb.ProjectAdded>, 

        IHandle<UI.ProjectDisplayed>,
        IHandle<UI.InboxDisplayed>,
        IHandle<UI.DragStarted<StuffId>>

    {
        readonly INavigationView _tree;
        
        readonly IMessageQueue _queue;
        readonly ClientPerspective _perspective;

        DragController _drag;

        bool _loaded;
        string _currentNode;


        NavigationController(INavigationView view, IMessageQueue queue, ClientPerspective perspective, DragController controller)
        {
            _tree = view;
            _tree.WhenInboxSelected(HandleInboxSelected);
            _tree.WhenProjectSelected(HandleProjectSelected);
            _queue = queue;
            _perspective = perspective;

            _drag = controller;
        }

        void HandleProjectSelected(string id)
        {
            if (id == _currentNode)
                return;
            _currentNode = id;

            _queue.Enqueue(new UI.DisplayProject(_projectKeys[id]));
        }

        void HandleInboxSelected()
        {
            if ("inbox" == _currentNode)
                return;
            _currentNode = "inbox";


            _queue.Enqueue(new UI.DisplayInbox());
        }

        readonly IDictionary<string,ProjectId> _projectKeys = new Dictionary<string, ProjectId>(); 

        public static NavigationController Wire(INavigationView view, IMessageQueue queue, ISubscriber bus, ClientPerspective model)
        {
            var drag = new DragController(queue);

            var adapter  = new NavigationController(view, queue, model, drag);
            
            bus.Subscribe<Dumb.StuffAddedToInbox>(adapter);
            bus.Subscribe<Dumb.StuffRemovedFromInbox>(adapter);
            bus.Subscribe<Dumb.ProjectAdded>(adapter);
            
            bus.Subscribe<Dumb.ClientModelLoaded>(adapter);
            
            bus.Subscribe<UI.InboxDisplayed>(adapter);
            bus.Subscribe<UI.ProjectDisplayed>(adapter);
            bus.Subscribe<UI.DragStarted<StuffId>>(adapter);

            view.SubscribeDragOver(drag);

            return adapter ;
        }

        public void Handle(Dumb.StuffAddedToInbox message)
        {
            if (!_loaded) return;

            _tree.UpdateInboxNode(message.InboxCount);
        }

        public void Handle(Dumb.StuffRemovedFromInbox message)
        {
            if (!_loaded) return;

            _tree.UpdateInboxNode(message.InboxCount);
        }
        
        public void Handle(Dumb.ClientModelLoaded message)
        {
            _loaded = true;

            _tree.UpdateInboxNode(message.Model.Inbox.Count);
            _tree.ReloadProjectList(message.Model.Projects.Select(p => p.Info).ToList());

            foreach (var key in message.Model.Projects)
            {
                _projectKeys[key.Info.UIKey] = key.Info.ProjectId;
            }
        }

        public void Handle(Dumb.ProjectAdded message)
        {
            if (!_loaded)
                return;

            _projectKeys[message.UniqueKey] = message.ProjectId;

            _tree.ReloadProjectList(_perspective.Model.ListProjects().Select(p => p.Info).ToList());
        }


        public void Handle(UI.ProjectDisplayed message)
        {
            if (_currentNode == message.Project.UIKey)
                return;

            _currentNode = message.Project.UIKey;
            _tree.SelectProject(message.Project.UIKey);
        }

        public void Handle(UI.InboxDisplayed message)
        {
            if (_currentNode == "inbox")
                return;

            _currentNode = "inbox";
            _tree.SelectInbox();
        }

        public void Handle(UI.DragStarted<StuffId> message)
        {
            _drag.WhenDraggingStuff(message.Request, message.Subject);
            
        }
    }
}