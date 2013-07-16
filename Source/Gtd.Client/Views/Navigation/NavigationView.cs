using System.Collections.Generic;
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

        public void AddOrUpdateNode(string key, string text)
        {
            TreeNode node;
            if (!_nodes.TryGetValue(key, out node))
            {
                node = new TreeNode(text) {Tag = key};
                _nodes[key] = node;
                treeView1.Nodes.Add(node);
            }
            else
            {
                node.Text = text;
            }
        }
    }
}
