using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gtd.Client.Views.Navigation
{
    public partial class NavigationView : UserControl
    {
        public NavigationView(NavigationAdapter navigationAdapter)
        {
            InitializeComponent();

            treeView1.AfterSelect += (sender, args) =>
                {
                    if (args.Node != null)
                    {
                        navigationAdapter.WhenNodeSelected((string)args.Node.Tag);
                    }
                };
        }

        IDictionary<string,TreeNode> _nodes = new Dictionary<string, TreeNode>(); 

        public void AddNode(string key, string text)
        {
            var treeNode = treeView1.Nodes.Add(key, text);
            treeNode.Tag = key;
            _nodes[key] = treeNode;
        }

        public void UpdateNode(string key, string text)
        {
            _nodes[key].Text = text;
        }
    }
}
