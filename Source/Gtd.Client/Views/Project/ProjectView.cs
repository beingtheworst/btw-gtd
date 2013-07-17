using System.Windows.Forms;
using Gtd.Client.Models;

namespace Gtd.Client.Views.Project
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        public void DisplayProject(FilteredProject project)
        {
            _actionList.BeginUpdate();

            _projectName.Text = string.Format("{0} ({1})", project.Outcome, project.ActionCount);
            try
            {
                _actionList.Items.Clear();
                foreach (var action in project.FilteredActions)
                {
                    _actionList.Items.Add(new ActionDisplay(action), action.Completed);
                }
            }
            finally
            {
                _actionList.EndUpdate();
            }
        }

        sealed class ActionDisplay
        {
            public readonly ImmutableAction Model;

            public ActionDisplay(ImmutableAction model)
            {
                Model = model;
            }

            public override string ToString()
            {
                return Model.Outcome;
            }
        }

        public void AttachTo(ProjectController controller)
        {
            _actionList.ItemCheck += (sender, args) =>
                {
                    var display = (ActionDisplay) _actionList.Items[args.Index];
                    if (args.NewValue == CheckState.Checked)
                    {
                        controller.RequestActionCheck(display.Model.ActionId);
                    }
                    else
                    {
                        // we don't support unchecks for now
                        _actionList.SetItemChecked(args.Index,true);
                        //adapter.RequestActionUncheck(display.View.Id);
                    }
                };
        }
    }
}
