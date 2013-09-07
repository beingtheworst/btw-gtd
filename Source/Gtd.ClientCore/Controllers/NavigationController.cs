using System;
using System.Collections.Generic;
using Gtd.Client.Models;
using System.Linq;
using Gtd.ClientCore;

namespace Gtd.Client.Views.Navigation
{
    // in the WinForms sample client, this interface is implemented
    // with a "Tree View" WinForms control that is similar to the Windows File Explorer
    public interface INavigationView
    {
        void UpdateInboxNode(int count);
        void ReloadProjectList(IList<ImmutableProjectInfo> projects);
        

        void WhenInboxSelected(Action action);
        void WhenProjectSelected(Action<string> project);

        void EnableDropSites(DragManager controller);
        void DisableDropSites();
        
        void SelectProject(string uiKey);
        void SelectInbox();
    }


    public sealed class NavigationController : 
        IHandle<Dumb.ClientModelLoaded>,
        IHandle<Dumb.StuffAddedToInbox>, 
        IHandle<Dumb.StuffRemovedFromInbox>,
        IHandle<Dumb.ProjectAdded>, 

        IHandle<UI.ProjectDisplayed>,
        IHandle<UI.InboxDisplayed>,
        IHandle<UI.DragStarted>, 
        IHandle<UI.DragCompleted>

    {
        readonly INavigationView _view;
        
        readonly IMessageQueue _queue;
        readonly ClientPerspective _perspective;


        bool _loaded;
        string _currentNode;


        NavigationController(INavigationView view, IMessageQueue queue, ClientPerspective perspective)
        {
            _view = view;
            _view.WhenInboxSelected(HandleInboxSelected);
            _view.WhenProjectSelected(HandleProjectSelected);
            _queue = queue;
            _perspective = perspective;

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

            var adapter  = new NavigationController(view, queue, model);
            
            bus.Subscribe<Dumb.StuffAddedToInbox>(adapter);
            bus.Subscribe<Dumb.StuffRemovedFromInbox>(adapter);
            bus.Subscribe<Dumb.ProjectAdded>(adapter);
            
            bus.Subscribe<Dumb.ClientModelLoaded>(adapter);
            
            bus.Subscribe<UI.InboxDisplayed>(adapter);
            bus.Subscribe<UI.ProjectDisplayed>(adapter);
            bus.Subscribe<UI.DragStarted>(adapter);            
            bus.Subscribe<UI.DragCompleted>(adapter);
            return adapter ;
        }

        public void Handle(Dumb.StuffAddedToInbox message)
        {
            if (!_loaded) return;

            _view.UpdateInboxNode(message.InboxCount);
        }

        public void Handle(Dumb.StuffRemovedFromInbox message)
        {
            if (!_loaded) return;

            _view.UpdateInboxNode(message.InboxCount);
        }
        
        public void Handle(Dumb.ClientModelLoaded message)
        {
            _loaded = true;

            _view.UpdateInboxNode(message.Model.Inbox.Count);
            _view.ReloadProjectList(message.Model.Projects.Select(p => p.Info).ToList());

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

            _view.ReloadProjectList(_perspective.Model.ListProjects().Select(p => p.Info).ToList());
        }


        public void Handle(UI.ProjectDisplayed message)
        {
            if (_currentNode == message.Project.UIKey)
                return;

            _currentNode = message.Project.UIKey;
            _view.SelectProject(message.Project.UIKey);
        }

        public void Handle(UI.InboxDisplayed message)
        {
            if (_currentNode == "inbox")
                return;

            _currentNode = "inbox";
            _view.SelectInbox();
        }

        public void Handle(UI.DragStarted message)
        {
            _view.EnableDropSites(message.Manager);
        }
        public void Handle(UI.DragCompleted e)
        {
            _view.DisableDropSites();
        }
    }
}