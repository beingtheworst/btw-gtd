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
        void SelectProject(string uiKey);
        void SelectInbox();
    }

    public sealed class NavigationController : 
        IHandle<AppInit>, 
        IHandle<Dumb.ClientModelLoaded>,
        IHandle<Dumb.StuffAddedToInbox>, 
        IHandle<Dumb.StuffRemovedFromInbox>,
        IHandle<Dumb.ProjectAdded>, 
        IHandle<UI.ProjectDisplayed>,
        IHandle<UI.InboxDisplayed>

    {
        readonly NavigationView _tree;
        readonly Region _region;
        readonly IPublisher _queue;
        readonly ClientPerspective _perspective;
        

        bool _loaded;

        NavigationController(Region region, IPublisher queue, ClientPerspective perspective)
        {
            _tree = new NavigationView();

            _tree.WhenInboxSelected(() =>
                {
                    if ("inbox" == _currentNode)
                        return;
                    _currentNode = "inbox";

                    
                    _queue.Publish(new UI.DisplayInbox());
                });
            _tree.WhenProjectSelected(id =>
                {
                    if (id == _currentNode)
                        return;
                    _currentNode = id;

                    _queue.Publish(new UI.DisplayProject(_projectKeys[id]));
                });

            

            _region = region;
            _queue = queue;
            _perspective = perspective;
        }

        IDictionary<string,ProjectId> _projectKeys = new Dictionary<string, ProjectId>(); 

        public static NavigationController Wire(Region control, IPublisher queue, ISubscriber bus, ClientPerspective view)
        {
            var adapter  = new NavigationController(control, queue, view);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<Dumb.StuffAddedToInbox>(adapter);
            bus.Subscribe<Dumb.StuffRemovedFromInbox>(adapter);
            bus.Subscribe<Dumb.ProjectAdded>(adapter);
            
            bus.Subscribe<Dumb.ClientModelLoaded>(adapter);
            
            bus.Subscribe<UI.InboxDisplayed>(adapter);
            bus.Subscribe<UI.ProjectDisplayed>(adapter);



            return adapter ;
        }

        public void Handle(AppInit message)
        {
            _region.RegisterDock(_tree, "nav-tree");
            _region.SwitchTo("nav-tree");
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

        string _currentNode;

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
    }
}