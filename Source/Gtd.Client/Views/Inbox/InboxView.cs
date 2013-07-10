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
            public readonly ThoughtId Id;
            public Thought(string name, ThoughtId id)
            {
                Name = name;
                Id = id;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        readonly IDictionary<ThoughtId, Thought> _thoughts = new Dictionary<ThoughtId, Thought>(); 

        public void AddThought(string thought, ThoughtId thoughtId)
        {
            var t = new Thought(thought, thoughtId);
            _thoughts.Add(thoughtId, t);
            listBox1.Items.Add(t);
            listBox1.Visible = listBox1.Items.Count > 0;
        }

        public void RemoveThought(ThoughtId thought)
        {
            Thought t;
            if (_thoughts.TryGetValue(thought, out t))
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

        ThoughtId[] GetSelectedThoughtIds()
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

        public void LoadThoughts(IList<ThoughtView> listInbox)
        {
            listBox1.BeginUpdate();
            try
            {
                foreach (var view in listInbox)
                {
                    var t = new Thought(view.Subject, view.Id);
                    _thoughts.Add(view.Id, t);
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
