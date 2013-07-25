using System;
using System.Windows.Forms;

namespace Gtd.Client.Views.AddStuffToInbox
{
    public partial class AddStuffToInboxForm : Form, IAddStuffToInboxWizard
    {
        readonly MainForm _parent;

        public AddStuffToInboxForm(MainForm parent)
        {
            _parent = parent;
            InitializeComponent();
        }


        public void TryGetUserInput(Action<string> future)
        {
            // Use the CaptureThoughtForm to try and get user input
            // on the same UI (Single Apartment) thread as the main parent form.
            // If it is able to, call back the Action that was passed in future above.
            _parent.Sync(() =>
                {
                    textBox1.Text = null;
                    if (ShowDialog(_parent) != DialogResult.OK) return;
                    var text = textBox1.Text;
                    future(text);
                });
        }

        private void AddStuffToInboxForm_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _ok.Enabled = !string.IsNullOrWhiteSpace(textBox1.Text);
        }
    }
}
