using System;
using System.Windows.Forms;

namespace Gtd.Client
{
    public sealed class TreeViewController : IHandle<AppInit>, IHandle<ThoughtCaptured>, IHandle<ThoughtArchived>
    {
        readonly TreeView _tree;
        readonly IPublisher _queue;
        TreeNode _treeNode;

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
        }

        public void Handle(AppInit message)
        {
            _tree.Invoke(new Action(InitTreeView));
        }

        void InitTreeView()
        {
            _treeNode = _tree.Nodes.Add("inbox", "Inbox");
        }

        void UpdateInboxNode(int count)
        {
            _treeNode.Text = string.Format("Inbox ({0})", count);
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
    }



    public interface IFormCommand
    {
        
    }
}