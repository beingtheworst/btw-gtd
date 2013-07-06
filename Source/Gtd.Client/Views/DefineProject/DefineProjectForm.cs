using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Gtd.Client
{
    public partial class DefineProjectForm : Form
    {
        readonly Form _parent;

        public DefineProjectForm(Form parent)
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            _ok.Enabled = !string.IsNullOrWhiteSpace(textBox1.Text);
        }

        private void DefineProjectForm_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }
    }
}
