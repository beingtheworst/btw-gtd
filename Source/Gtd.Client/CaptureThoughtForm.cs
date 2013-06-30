using System;
using System.Windows.Forms;

namespace Gtd.Client
{
    public partial class CaptureThoughtForm : Form
    {
        public CaptureThoughtForm()
        {
            InitializeComponent();
        }

        public static string TryGetUserInput(IWin32Window owner)
        {
            using (var f = new CaptureThoughtForm())
            {
                if (f.ShowDialog(owner) == DialogResult.OK)
                {
                    return f.textBox1.Text;
                }
                return null;
            }
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
