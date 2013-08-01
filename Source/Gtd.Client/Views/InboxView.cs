using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gtd.Client.Models;

namespace Gtd.Client
{
    public partial class InboxView : UserControl, IInboxView
    {
        //readonly InboxController _controller;

        public InboxView()
        {
            
            InitializeComponent();

            _toProject.Enabled = false;
        }



        sealed class StuffInfo
        {
            readonly ImmutableStuff _stuff;
            public StuffId Id { get { return _stuff.StuffId; } }
            
            public StuffInfo(ImmutableStuff stuff)
            {
                _stuff = stuff;
            
            }

            public override string ToString()
            {
                return _stuff.Description;
            }
        }

        readonly IDictionary<StuffId, StuffInfo> _stuffInInbox = new Dictionary<StuffId, StuffInfo>(); 

      
        

        
        StuffId[] GetSelectedStuffIds()
        {
            return listBox1.SelectedItems.Cast<StuffInfo>().Select(stuffItem => stuffItem.Id).ToArray();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = listBox1.SelectedIndices.Count > 0;
            _toProject.Enabled = listBox1.SelectedIndices.Count > 0;
        }

    
        
        void _toProject_DropDown(object sender, EventArgs e)
        {

            _toProject.BeginUpdate();
            try
            {
                _toProject.Items.Clear();
                var projects = _whenListProjects();
                foreach (var info in projects)
                {
                    _toProject.Items.Add(new Display(info.ProjectId, info.Outcome));
                }
            }
            finally
            {
                _toProject.EndUpdate();
            }
        }


       

        public void LoadInbox(ImmutableInbox inbox)
        {
            listBox1.BeginUpdate();
            try
            {
                listBox1.Items.Clear();
                foreach (var view in inbox.Stuff)
                {
                    var stuffInfo = new StuffInfo(view);
                    _stuffInInbox[view.StuffId] = stuffInfo;
                    listBox1.Items.Add(stuffInfo);
                }
                listBox1.Visible = listBox1.Items.Count > 0;
            }
            finally
            {
                listBox1.EndUpdate();
            }
        }

        public void AddStuff(ImmutableStuff stuff)
        {
            var stuffInfo = new StuffInfo(stuff);
            _stuffInInbox.Add(stuff.StuffId, stuffInfo);
            this.Sync(() =>
                {
                    listBox1.Items.Add(stuffInfo);
                    listBox1.Visible = listBox1.Items.Count > 0;
                });
        }

        public void RemoveStuff(StuffId stuffId)
        {
            StuffInfo stuffInfo;

            if (!_stuffInInbox.TryGetValue(stuffId, out stuffInfo)) return;


            _stuffInInbox.Remove(stuffInfo.Id);
            this.Sync(() =>
                {
                    listBox1.Items.Remove(stuffInfo);
                    listBox1.Visible = listBox1.Items.Count > 0;
                });
        }

       
        Region _region;

        public void AttachTo(Region mainRegion)
        {
            _region = mainRegion;
            mainRegion.RegisterDock(this, "inbox");
        }

        

        public void SubscribeToTrashStuffClick(Action<StuffId[]> callback)
        {
            button1.Click += (sender, args) => callback(GetSelectedStuffIds());
        }

        public void SubscribeToAddStuffClick(Action callback)
        {
            _addStuff.Click += (sender, args) => callback();
        }

        Func<ICollection<ImmutableProjectInfo>> _whenListProjects = () => { throw new InvalidOperationException(); }; 

        public void SubscribeToListProjects(Func<ICollection<ImmutableProjectInfo>> request)
        {
            _whenListProjects = request;
        }

        public void SubscribeToMoveStuffToProject(Action<ProjectId, StuffId[]> callback)
        {
            _toProject.SelectionChangeCommitted += (sender, args) =>
                {
                    var projectId = ((ProjectId) (((Display) _toProject.SelectedItem).Value));
                    callback(projectId, GetSelectedStuffIds());
                };

        }

        public void SubscribeToStartDrag(Action<string,StuffId> callback)
        {
            listBox1.MouseDown += (sender, e) =>
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        var index = listBox1.IndexFromPoint(e.X, e.Y);
                        if (index >= 0)
                        {
                            var request = Guid.NewGuid().ToString();
                            var item = (StuffInfo) listBox1.Items[index];
                            DoDragDrop(request, DragDropEffects.Move);
                            callback(request,item.Id);

                        }
                    }
                };
        }

        public void ShowInbox(ImmutableInbox inbox)
        {
            this.Sync(() => LoadInbox(inbox));
            _region.SwitchTo("inbox");

        }

      
    }

    public sealed class Display
    {
        public ProjectId Value;
        public string Text;

        public Display(ProjectId value, string text)
        {
            Value = value;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
