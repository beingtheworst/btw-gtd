using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gtd.CoreDomain;
using Gtd.Shell;
using Gtd.Shell.Projections;

namespace Gtd.Client
{
    public partial class Form1 : Form, IMainDock
    {
        public Form1()
        {
            InitializeComponent();


            


            

            // wire in all controllers




        }




        
     
        private void Form1_Load(object sender, EventArgs e)
        {
            // when we are loading the form for the 1st time
            


        }

        IDictionary<string, UserControl> _panels = new Dictionary<string, UserControl>();
        string _activePanel = null;

        public void RegisterDock(UserControl control, string key)
        {
            
            var panel = splitContainer1.Panel2;

            panel.Invoke(new Action(() =>
                {
                    control.Visible = false;
                    control.Dock = DockStyle.Fill;
                    panel.Controls.Add(control);

                    _panels.Add(key, control);
                }));

            
        }

        public void SwitchTo(string key)
        {
            var panel = splitContainer1.Panel2;
            panel.Invoke(new Action(() =>
                {
                    if (_activePanel != null)
                    {
                        _panels[_activePanel].Visible = false;
                    }
                    _activePanel = key;
                    _panels[_activePanel].Visible = true;
                }));
        }

        
    }

    public sealed class MainFormController : IHandle<AppInit>
    {
        Form1 _mainForm;
        IPublisher _queue;

        public MainFormController(Form1 mainForm, IPublisher queue)
        {
            _mainForm = mainForm;
            _queue = queue;

            _mainForm.Load += (sender, args) => _queue.Publish(new FormLoad());
            _mainForm.captureToolStripMenuItem.Click += (sender, args) => _queue.Publish(new RequestCapture());
        }

        public void Handle(AppInit message)
        {
            
        }
    }

    public sealed class RequestCapture : Message {}


    public sealed class RequestRemove : Message
    {
        public readonly ThoughtId Id;

        public RequestRemove(ThoughtId id)
        {
            Id = id;
        }
    }



    public enum AppState
    {
        Loading,
        InboxView
    }



    



    public enum UIState
    {
        Loading,
        Inbox
    }

    


}
