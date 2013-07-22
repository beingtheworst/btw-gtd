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

        sealed class Thought
        {
            
            public readonly string Name;
            public readonly InboxStuffId Id;
            public Thought(string name, InboxStuffId id)
            {
                Name = name;
                Id = id;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        readonly IDictionary<InboxStuffId, Thought> _thoughts = new Dictionary<InboxStuffId, Thought>(); 

        public void AddThought(string thought, InboxStuffId inboxStuffId)
        {
            var t = new Thought(thought, inboxStuffId);
            _thoughts.Add(inboxStuffId, t);
            listBox1.Items.Add(t);
            listBox1.Visible = listBox1.Items.Count > 0;
        }

        public void RemoveThought(InboxStuffId inboxStuff)
        {
            Thought t;
            if (_thoughts.TryGetValue(inboxStuff, out t))
            {
                listBox1.Items.Remove(t);
                _thoughts.Remove(t.Id);
            }
            listBox1.Visible = listBox1.Items.Count > 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _controller.WhenRequestedThoughtsArchival(GetSelectedThoughtIds());
        }

        InboxStuffId[] GetSelectedThoughtIds()
        {
            return listBox1.SelectedItems.Cast<Thought>().Select(t => t.Id).ToArray();
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
            var id = ((ProjectId)(((Display)_toProject.SelectedItem).Value));
            _controller.WhenRequestedMoveThoughtsToProject(id, GetSelectedThoughtIds());
        }

        private void _capture_Click(object sender, EventArgs e)
        {
            _controller.WhenCaptureThoughtClicked();
        }

        public void LoadInbox(ImmutableInbox inbox)
        {
            listBox1.BeginUpdate();
            try
            {
                foreach (var view in inbox.Thoughts)
                {
                    var t = new Thought(view.Subject, view.InboxStuffId);
                    _thoughts.Add(view.InboxStuffId, t);
                    listBox1.Items.Add(t);
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
