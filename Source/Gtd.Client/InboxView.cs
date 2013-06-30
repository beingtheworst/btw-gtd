using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Gtd.Client
{
    public partial class InboxView : UserControl
    {

        IPublisher _sink;
        public InboxView(IPublisher sink)
        {
            _sink = sink;
            InitializeComponent();
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

        IDictionary<ThoughtId, Thought> _thoughts = new Dictionary<ThoughtId, Thought>(); 

        public void AddThought(string thought, ThoughtId thoughtId)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => AddThought(thought, thoughtId)));
                return;
            }
            var t = new Thought(thought, thoughtId);
            _thoughts.Add(thoughtId, t);
            listBox1.Items.Add(t);
        }

        public void RemoveThought(ThoughtId thought)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => RemoveThought(thought)));
                return;
            }

            Thought t;
            if (_thoughts.TryGetValue(thought, out t))
            {
                listBox1.Items.Remove(t);
                _thoughts.Remove(t.Id);
            }
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (Thought index in listBox1.SelectedItems)
            {
                _sink.Publish(new RequestRemove(index.Id));
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = listBox1.SelectedIndices.Count > 0;
        }

    }

    public class InboxViewController : 
        IHandle<AppInit>, IHandle<ShowInbox>, IHandle<ThoughtCaptured>,  IHandle<ThoughtArchived>

    {
        readonly IMainDock _dock;
        readonly IPublisher _queue;

        readonly InboxView _view;

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<ShowInbox>(this);
            bus.Subscribe<ThoughtCaptured>(this);
            bus.Subscribe<ThoughtArchived>(this);
        }

        public InboxViewController(IMainDock dock, IPublisher queue)
        {
            _dock = dock;
            _queue = queue;
            _view = new InboxView(queue);
            
        }

        public void Handle(AppInit message)
        {
            _dock.RegisterDock(_view, "inbox");
        }

        public void Handle(ShowInbox message)
        {
            _dock.SwitchTo("inbox");
            _queue.Publish(new InboxShown());

        }

        public void Handle(ThoughtCaptured message)
        {
            _view.AddThought(message.Thought, message.ThoughtId);
        }
        public void Handle(ThoughtArchived message)
        {
            _view.RemoveThought(message.ThoughtId);
        }


    }

    public class ShowInbox : Message
    {
        
    }

    public class InboxShown : Message
    {
        
    }

}
