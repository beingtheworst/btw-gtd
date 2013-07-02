using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Gtd.Client
{
    public partial class InboxView : UserControl
    {
        readonly IPublisher _sink;
        readonly ISystemView _view;

        public InboxView(IPublisher sink, ISystemView view)
        {
            _sink = sink;
            _view = view;
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
            foreach (Thought index in listBox1.SelectedItems)
            {
                _sink.Publish(new RequestArchiveThought(index.Id));
            }
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
                foreach (var info in _view.ListProjects())
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

            var thoughtIds = listBox1.SelectedItems.Cast<Thought>().Select(t => t.Id).ToArray();
            _sink.Publish(new RequestMoveThoughtsToProject(thoughtIds, id));
        }

        private void _capture_Click(object sender, EventArgs e)
        {
            _sink.Publish(new CaptureThoughtClicked());
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


    public class RequestShowInbox : Message
    {
        
    }

    public class InboxShown : Message
    {
        
    }

}
