using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gtd.Client.Views.Navigation;

namespace Gtd.Client
{
    public sealed class NavigationAdapter : 
        IHandle<AppInit>, 
        IHandle<FormLoaded>,
    IHandle<ThoughtCaptured>, 
        IHandle<ThoughtArchived>,
        IHandle<ProjectDefined>, IHandle<ActionDefined>
    {
        readonly NavigationView _tree;
        readonly Region _region;
        readonly IPublisher _queue;
        readonly ISystemView _view;
        

        bool _visible;

        NavigationAdapter(Region region, IPublisher queue, ISystemView view)
        {
            _tree = new NavigationView(this);
            _region = region;
            _queue = queue;
            _view = view;
        }

        public static NavigationAdapter Wire(Region control, IPublisher queue, ISubscriber bus, ISystemView view)
        {
            var adapter  = new NavigationAdapter(control, queue, view);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<ThoughtCaptured>(adapter);
            bus.Subscribe<ThoughtArchived>(adapter);
            bus.Subscribe<ProjectDefined>(adapter);
            bus.Subscribe<ActionDefined>(adapter);
            bus.Subscribe<FormLoaded>(adapter);



            return adapter ;
        }

        public void Handle(AppInit message)
        {
            _region.RegisterDock(_tree, "nav-tree");
            _region.SwitchTo("nav-tree");

            _tree.Sync(() => _tree.AddNode("inbox","Inbox"));
        }

        

        void UpdateInboxNode()
        {
            _tree.UpdateNode("inbox",string.Format("Inbox ({0})", _view.ListInbox().Length));
        }

        

        public void Handle(ThoughtCaptured message)
        {
            if (!_visible)
                return;

            Sync(UpdateInboxNode);
        }

        public void Handle(ThoughtArchived message)
        {
            if (!_visible)
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


        IDictionary<ProjectId, string> _projectNodes = new Dictionary<ProjectId, string>(); 

        

        public void Handle(ProjectDefined message)
        {
            if (!_visible)
                return;
            Sync(() => AddProjectNode(message.ProjectId, message.ProjectOutcome));
        }

        void AddProjectNode(ProjectId projectId, string outcome)
        {
            var key = "project|" + projectId.ToString();
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

        public void Handle(FormLoaded message)
        {
            _visible = true;

            Sync(() =>
                {
                    foreach (var project in _view.ListProjects())
                    {
                        AddProjectNode(project.ProjectId, string.Format("{0} ({1})", project.Outcome, project.Actions.Count));
                    }
                    UpdateInboxNode();
                });

        }

        public void WhenNodeSelected(string tag)
        {
            var node = _nodes[tag];

            if (node is ProjectId)
            {
                _queue.Publish(new RequestShowProject((ProjectId) node));
            }
        }
    }



    public interface IFormCommand
    {
        
    }
}