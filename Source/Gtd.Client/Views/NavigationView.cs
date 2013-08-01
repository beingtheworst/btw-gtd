using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Gtd.Client.Models;

namespace Gtd.Client.Views.Navigation
{
    public partial class NavigationView : UserControl, INavigationView
    {
        public NavigationView()
        {
            InitializeComponent();

            _inboxNode = treeView1.Nodes.Add("inbox", "Inbox", "inbox");

            treeView1.AfterSelect += (sender, args) => OnSelectNode(args.Node);
        }

        void OnSelectNode(TreeNode node)
        {
            if (null == node)
                return;
            if (node == _inboxNode)
            {
                _whenInboxSelected();
            }
            else
            {
                _whenProjectSelected(node.Name);
            }
        }

        public void UpdateInboxNode(int count)
        {
            treeView1.Sync(() => _inboxNode.Text = string.Format("Inbox ({0})", count));
        }

        public void ReloadProjectList(IList<ImmutableProjectInfo> projects)
        {
            treeView1.Sync(() =>
                {
                    treeView1.BeginUpdate();
                    try
                    {
                        while (treeView1.Nodes.Count>1)
                        treeView1.Nodes.RemoveAt(1);
                        
                        foreach (var p in projects)
                        {
                            var node = treeView1.Nodes.Add(p.UIKey, p.Outcome, "project", "project");
                            node.Tag = p.ProjectId;
                        }
                    }
                    finally
                    {
                        treeView1.EndUpdate();
                    }
                    
                });
        }

        Action _whenInboxSelected = () =>
            {
                throw new InvalidOperationException("WhenInboxSelected is not assigned");
            };

        Action<string> _whenProjectSelected = id =>
            {
                throw new InvalidOperationException("WhenProjectSelected is not assigned");
            };

        readonly TreeNode _inboxNode;


        public void WhenInboxSelected(Action action)
        {
            _whenInboxSelected = action;
        }

        public void WhenProjectSelected(Action<string> project)
        {
            _whenProjectSelected = project;
        }

        public void SubscribeDragOver(DragController controller)
        {
            treeView1.DragDrop += (sender, e) =>
                {
                    var point = treeView1.PointToClient(new Point(e.X, e.Y));
                    var node = treeView1.GetNodeAt(point);
                    if (null == node)
                        return;

                    if (node.Tag is ProjectId)
                    {
                        var data = (string)e.Data.GetData(DataFormats.StringFormat);
                        controller.DropToProject(data, (ProjectId) node.Tag);
                    }
                    

                };
            treeView1.DragOver += (sender, e) =>
                {
                    var point = treeView1.PointToClient(new Point(e.X, e.Y));
                    var node = treeView1.GetNodeAt(point);
                    if (null == node)
                        return;

                    if (node.Tag is ProjectId)
                    {
                        var data = (string)e.Data.GetData(DataFormats.StringFormat);
                        if (controller.CanAcceptDragOverProject(data))
                        {
                            e.Effect = DragDropEffects.Move;
                        }
                        else
                        {
                            e.Effect = DragDropEffects.None;
                        }
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }

                    //
                    
                };
        }

        public void SelectProject(string uiKey)
        {
            this.Sync(() =>
                {
                    var node = treeView1.Nodes[uiKey];
                    if (null != node && !node.IsSelected)
                    {
                        treeView1.SelectedNode = node;
                    }
                });
        }

        public void SelectInbox()
        {
            this.Sync(() =>
                {
                    if (!_inboxNode.IsSelected)
                    {
                        treeView1.SelectedNode = _inboxNode;
                    }
                });
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
           
        }
    }

    public abstract class UiElement
    {
        
    }
}
