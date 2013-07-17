using System;
using System.Windows.Forms;
using Gtd.Client.Models;
using System.Linq;

namespace Gtd.Client.Views.Project
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();

            _grid.DataSource = source;

        }

        BindingSource source = new BindingSource();

        public void DisplayProject(FilteredProject project)
        {
            
            
            

            _projectName.Text = string.Format("{0} ({1})", project.Outcome, project.ActionCount);

            source.Clear();
            foreach (var action in project.FilteredActions)
            {
                source.Add(new ActionDisplay(action,_controller));
            }
            
            
            //_grid.DataSource = list;
            
        }

        public sealed class ActionDisplay
        {
            public readonly ImmutableAction Model;
            readonly ProjectController _controller;

            public ActionDisplay(ImmutableAction model, ProjectController controller)
            {
                Model = model;
                _controller = controller;
            }


            public string Outcome
            {
                get { return Model.Outcome; }
                set { _controller.RequestOutcomeChange(Model.ActionId, value);}
            }

            public bool Completed
            {
                get { return Model.Completed; }
                set
                {
                    if (value)
                        _controller.RequestActionCheck(Model.ActionId);
                }
            }

            public override string ToString()
            {
                return Model.Outcome;
            }
        }

        ProjectController _controller;
        public void AttachTo(ProjectController controller)
        {
            _controller = controller;
            //_actionList.ItemCheck += (sender, args) =>
            //    {
            //        var display = (ActionDisplay) _actionList.Items[args.Index];
            //        if (args.NewValue == CheckState.Checked)
            //        {
            //            controller.RequestActionCheck(display.Model.ActionId);
            //        }
            //        else
            //        {
            //            // we don't support unchecks for now
            //            _actionList.SetItemChecked(args.Index,true);
            //            //adapter.RequestActionUncheck(display.View.Id);
            //        }
            //    };
        }
    }
}
