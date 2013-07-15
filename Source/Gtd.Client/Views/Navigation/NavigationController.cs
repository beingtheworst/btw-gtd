using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gtd.Client.Models;
using Gtd.Client.Views.Navigation;
using System.Linq;

namespace Gtd.Client
{
    public sealed class NavigationController : 
        IHandle<AppInit>, 
        IHandle<Cm.ClientModelLoaded>,
    IHandle<ThoughtCaptured>, 
        IHandle<ThoughtArchived>,
        IHandle<ProjectDefined>, IHandle<ActionDefined>, IHandle<Ui.FilterChanged>
    {
        readonly NavigationView _tree;
        readonly Region _region;
        readonly IPublisher _queue;
        readonly ClientPerspective _view;
        

        bool _loaded;

        NavigationController(Region region, IPublisher queue, ClientPerspective view)
        {
            _tree = new NavigationView(this);
            _region = region;
            _queue = queue;
            _view = view;
        }

        public static NavigationController Wire(Region control, IPublisher queue, ISubscriber bus, ClientPerspective view)
        {
            var adapter  = new NavigationController(control, queue, view);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<ThoughtCaptured>(adapter);
            bus.Subscribe<ThoughtArchived>(adapter);
            bus.Subscribe<ProjectDefined>(adapter);
            bus.Subscribe<ActionDefined>(adapter);
            bus.Subscribe<Cm.ClientModelLoaded>(adapter);
            bus.Subscribe<Ui.FilterChanged>(adapter);


            return adapter ;
        }

        public void Handle(AppInit message)
        {
            _region.RegisterDock(_tree, "nav-tree");
            _region.SwitchTo("nav-tree");

            //_tree.Sync(() => _tree.AddNode("inbox","Inbox"));
        }

        

        void UpdateInboxNode()
        {
            _tree.UpdateNode("inbox",string.Format("Inbox ({0})", _view.ListInbox().Count));
        }

        

        public void Handle(ThoughtCaptured message)
        {
            if (!_loaded)
                return;

            Sync(UpdateInboxNode);
        }

        public void Handle(ThoughtArchived message)
        {
            if (!_loaded)
                return;
            Sync(UpdateInboxNode);
        }

        void Sync(Action act)
        {
            if (_tree.InvokeRequired)
            {
                _tree.Invoke(act);
                return;
            }
            act();
        }


        readonly IDictionary<ProjectId, string> _projectNodes = new Dictionary<ProjectId, string>(); 

        

        public void Handle(ProjectDefined message)
        {
            if (!_loaded)
                return;
            Sync(() => AddProjectNode(message.ProjectId, message.ProjectOutcome));
        }

        void AddProjectNode(ProjectId projectId, string outcome)
        {
            var key = ToKey(projectId);
            _tree.AddNode(key, outcome);
            _projectNodes[projectId] = key;
            _nodes[key] = projectId;
        }

        IDictionary<string,object> _nodes = new Dictionary<string, object>(); 

        public void Handle(ActionDefined message)
        {
            //Sync(() => 
            //    _v
            //    _projectNodes[message.ProjectId].Text);
        }

        static string ToKey(ProjectId id)
        {
            return "project|" + id.Id;
        }

        

        public void Handle(Cm.ClientModelLoaded message)
        {
            _loaded = true;

            Sync(LoadNavigation);

        }

        void LoadNavigation()
        {
            _tree.Clear();
            _tree.AddNode("inbox", string.Format("Inbox ({0})", _view.ListInbox().Count));
            foreach (var project in _view.ListProjects())
            {
                var actions = _view.CurrentFilter.FilterActions(project);
                var count = _view.CurrentFilter.FormatActionCount(actions.Count());
                AddProjectNode(project.ProjectId, string.Format("{0} ({1})", project.Outcome, count));
            }
        }

        public void WhenNodeSelected(string tag)
        {
            if (tag == "inbox")
            {
                _queue.Publish(new Ui.DisplayInbox());
                return;
            }
            var node = _nodes[tag];

            if (node is ProjectId)
            {
                _queue.Publish(new Ui.DisplayProject((ProjectId) node));
            }
        }

        public void Handle(Ui.FilterChanged message)
        {
            if (!_loaded) return;
            Sync(LoadNavigation);
        }
    }



    public interface IFormCommand
    {
        
    }
}