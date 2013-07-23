using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Gtd.Client.Models;

namespace Gtd.Client
{
    public partial class InboxView : UserControl
    {
        readonly InboxController _controller;

        public InboxView(InboxController controller)
        {
            _controller = controller;
            InitializeComponent();

            _toProject.Enabled = false;
        }

        sealed class StuffInfo
        {
            
            public readonly string Description;
            public readonly StuffId Id;
            public StuffInfo(string description, StuffId id)
            {
                Description = description;
                Id = id;
            }

            public override string ToString()
            {
                return Description;
            }
        }

        readonly IDictionary<StuffId, StuffInfo> _stuffInInbox = new Dictionary<StuffId, StuffInfo>(); 

        public void AddStuff(string stuffDescription, StuffId stuffId)
        {
            var stuffInfo = new StuffInfo(stuffDescription, stuffId);
            _stuffInInbox.Add(stuffId, stuffInfo);
            listBox1.Items.Add(stuffInfo);
            listBox1.Visible = listBox1.Items.Count > 0;
        }

        public void TrashStuff(StuffId stuffId)
        {
            StuffInfo stuffInfo;
            if (_stuffInInbox.TryGetValue(stuffId, out stuffInfo))
            {
                listBox1.Items.Remove(stuffInfo);
                _stuffInInbox.Remove(stuffInfo.Id);
            }
            listBox1.Visible = listBox1.Items.Count > 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _controller.WhenRequestedToTrashStuff(GetSelectedStuffIds());
        }

        StuffId[] GetSelectedStuffIds()
        {
            return listBox1.SelectedItems.Cast<StuffInfo>().Select(stuffItem => stuffItem.Id).ToArray();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = listBox1.SelectedIndices.Count > 0;
            _toProject.Enabled = listBox1.SelectedIndices.Count > 0;
        }

        private void _toProject_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        
        void _toProject_DropDown(object sender, EventArgs e)
        {

            _toProject.BeginUpdate();
            try
            {
                _toProject.Items.Clear();
                foreach (var info in _controller.ListProjects())
                {
                    _toProject.Items.Add(new Display(info.ProjectId, info.Outcome));
                }
            }
            finally
            {
                _toProject.EndUpdate();
            }
        }

        private void _toProject_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var projectId = ((ProjectId)(((Display)_toProject.SelectedItem).Value));
            _controller.WhenRequestedToMoveStuffToProject(projectId, GetSelectedStuffIds());
        }

        private void _capture_Click(object sender, EventArgs e)
        {
            _controller.WhenAddStuffClicked();
        }

        public void LoadInbox(ImmutableInbox inbox)
        {
            listBox1.BeginUpdate();
            try
            {
                foreach (var view in inbox.Stuff)
                {
                    var stuffInfo = new StuffInfo(view.Description, view.StuffId);
                    _stuffInInbox.Add(view.StuffId, stuffInfo);
                    listBox1.Items.Add(stuffInfo);
                }
                listBox1.Visible = listBox1.Items.Count > 0;

                

            }
            finally
            {
                listBox1.EndUpdate();
            }
        }
    }

    public sealed class Display
    {
        public object Value;
        public string Text;

        public Display(object value, string text)
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
