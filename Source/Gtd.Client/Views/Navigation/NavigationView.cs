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
        public NavigationView()
        {
            InitializeComponent();
        }

        IDictionary<string,TreeNode> _nodes = new Dictionary<string, TreeNode>(); 

        public void AddNode(string key, string text)
        {
            _nodes[key] = treeView1.Nodes.Add(key, text);
        }

        public void UpdateNode(string key, string text)
        {
            _nodes[key].Text = text;
        }
    }
}
