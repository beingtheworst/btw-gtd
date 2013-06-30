using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Gtd.Client
{
    public sealed class TreeViewController : 
        IHandle<AppInit>, 
        IHandle<ThoughtCaptured>, 
        IHandle<ThoughtArchived>,
        IHandle<ProjectDefined>
    {
        readonly TreeView _tree;
        readonly IPublisher _queue;
        TreeNode _inboxNode;
        

        public TreeViewController(TreeView tree, IPublisher queue)
        {
            _tree = tree;
            _queue = queue;
            
        }

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
            bus.Subscribe<ProjectDefined>(this);
        }

        public void Handle(AppInit message)
        {
            _tree.Invoke(new Action(InitTreeView));
        }

        void InitTreeView()
        {
            _inboxNode = _tree.Nodes.Add("inbox", "Inbox");
        }

        void UpdateInboxNode(int count)
        {
            _inboxNode.Text = string.Format("Inbox ({0})", count);
        }

        int thoughts = 0;

        public void Handle(ThoughtCaptured message)
        {
            thoughts += 1;
            _tree.Invoke(new Action(() => UpdateInboxNode(thoughts)));
        }

        public void Handle(ThoughtArchived message)
        {
            thoughts -= 1;

            _tree.Invoke(new Action(() => UpdateInboxNode(thoughts)));
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
            Sync(() =>
                {
                    var node = _tree.Nodes.Add(message.ProjectId.ToString(), message.ProjectOutcome);
                    _projectNodes[message.ProjectId] = node;
                });
        }
    }



    public interface IFormCommand
    {
        
    }
}