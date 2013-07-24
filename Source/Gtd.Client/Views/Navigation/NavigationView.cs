using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Gtd.Client.Views.Navigation
{
    public partial class NavigationView : UserControl
    {
        public NavigationView(NavigationController navigationController)
        {
            InitializeComponent();

            treeView1.AfterSelect += (sender, args) =>
                {
                    if (args.Node != null)
                    {
                        navigationController.WhenNodeSelected((string)args.Node.Tag);
                    }
                };
        }

        readonly IDictionary<string,TreeNode> _nodes = new Dictionary<string, TreeNode>(); 
        public void Clear()
        {
            _nodes.Clear();
            treeView1.Nodes.Clear();
        }

        public void AddOrUpdateNode(string key, string text, NodeType type)
        {
            TreeNode node;
            if (!_nodes.TryGetValue(key, out node))
            {
                var s = type.ToString().ToLowerInvariant();
                node = new TreeNode(text) {Tag = key,
                    ImageKey = s,
                    SelectedImageKey = s
                };
                _nodes[key] = node;
                treeView1.Nodes.Add(node);
            }
            else
            {
                node.Text = text;
            }
        }


        public void UpdateSelectionIfNeeded(string uiKey)
        {
            var node = _nodes[uiKey];
            if (!node.IsSelected)
                this.treeView1.SelectedNode = node;
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
        
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            //e.Effect = DragDropEffects.Move;
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {

            var client = treeView1.PointToClient(new Point(e.X, e.Y));
            var node = treeView1.HitTest(client.X, client.Y);


            

            if (null == node.Node)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Move;
            }
        }
    }

    public enum NodeType
    {
        Inbox,
        Project
    }
}
