using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Gtd.Client
{
    public sealed class NavigationViewController : 
        IHandle<AppInit>, 
        IHandle<FormLoaded>,
    IHandle<ThoughtCaptured>, 
        IHandle<ThoughtArchived>,
        IHandle<ProjectDefined>, IHandle<ActionDefined>
    {
        readonly TreeView _tree;
        readonly IPublisher _queue;
        readonly ISystemView _view;
        TreeNode _inboxNode;

        bool _visible;

        public NavigationViewController(TreeView tree, IPublisher queue, ISystemView view)
        {
            _tree = tree;
            _queue = queue;
            _view = view;
        }

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
            bus.Subscribe<ProjectDefined>(this);
            bus.Subscribe<ActionDefined>(this);
            bus.Subscribe<FormLoaded>(this);
        }

        public void Handle(AppInit message)
        {
            _tree.Invoke(new Action(InitTreeView));
        }

        void InitTreeView()
        {
            _inboxNode = _tree.Nodes.Add("inbox", "Inbox");
        }

        void UpdateInboxNode()
        {
            _inboxNode.Text = string.Format("Inbox ({0})", _view.ListInbox().Length);
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


        IDictionary<ProjectId, TreeNode> _projectNodes = new Dictionary<ProjectId, TreeNode>(); 

        

        public void Handle(ProjectDefined message)
        {
            if (!_visible)
                return;
            Sync(() => AddProjectNode(message.ProjectId, message.ProjectOutcome));
        }

        void AddProjectNode(ProjectId projectId, string outcome)
        {
            var node = _tree.Nodes.Add(projectId.ToString(), outcome);
            _projectNodes[projectId] = node;
        }

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
    }



    public interface IFormCommand
    {
        
    }
}