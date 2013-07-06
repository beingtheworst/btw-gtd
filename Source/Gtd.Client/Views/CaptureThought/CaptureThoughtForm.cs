using System;
using System.Windows.Forms;

namespace Gtd.Client
{
    public partial class CaptureThoughtForm : Form
    {
        readonly MainForm _parent;

        public CaptureThoughtForm(MainForm parent)
        {
            _parent = parent;
            InitializeComponent();
        }


        public void TryGetUserInput(Action<string> future)
        {
            _parent.Sync(() =>
                {
                    textBox1.Text = null;
                    if (ShowDialog(_parent) != DialogResult.OK) return;
                    var text = textBox1.Text;
                    future(text);
                });
        }

        private void CaptureThoughtForm_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _ok.Enabled = !string.IsNullOrWhiteSpace(textBox1.Text);
        }
    }
}
