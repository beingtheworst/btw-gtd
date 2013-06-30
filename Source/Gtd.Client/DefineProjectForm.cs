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
        public DefineProjectForm()
        {
            InitializeComponent();
        }

        public static string TryGetUserInput(IWin32Window owner)
        {
            using (var f = new DefineProjectForm())
            {
                if (f.ShowDialog(owner) == DialogResult.OK)
                {
                    return f.textBox1.Text;
                }
                return null;
            }
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
