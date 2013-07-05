using System;
using System.Windows.Forms;

namespace Gtd.Client
{
    public partial class MainForm : Form, ILogControl
    {
        MainFormController _controller;
        public readonly Region MainRegion;
        public readonly Region NavigationRegion;

        public MainForm()
        {
            InitializeComponent();

            MainRegion = new Region(splitContainer1.Panel2);
            NavigationRegion = new Region(splitContainer1.Panel1);
        }



        public void SetAdapter(MainFormController controller)
        {
            _controller = controller;

            Load += (sender, args) => _controller.Publish(new FormLoading());
            captureToolStripMenuItem.Click += (sender, args) => _controller.Publish(new CaptureThoughtClicked());

            projectToolStripMenuItem.Click += (sender, args) =>
            {
                var c = DefineProjectForm.TryGetUserInput(this);
                if (null != c) _controller.Publish(new Ui.DefineNewProject(c));
            };

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // when we are loading the form for the 1st time
        }

        public void HideGoToInbox()
        {
            this.Sync(() => goToInboxToolStripMenuItem.Visible = false);
        }

        public void ShowGoToInbox()
        {
            this.Sync(() => goToInboxToolStripMenuItem.Visible = true);
        }


        public void Log(string toString)
        {
            _log.Sync(() =>
                {
                    var format = string.Format("{0:HH:mm:ss} {1}{2}", DateTime.UtcNow, toString, Environment.NewLine);
                    _log.AppendText(format);
                    _log.ScrollToCaret();
                });
        }
    }


    public static class ExtendControl
    {
        public static void Sync(this Control self, Action act)
        {
            if (self.InvokeRequired)
            {
                self.Invoke(act);
            }
            else
            {
                act();
            }
        }
    }

   
   

    public sealed class CaptureThoughtClicked : Message
    {
        
    }







    public enum AppState
    {
        Loading,
        
    }



    




    


}
