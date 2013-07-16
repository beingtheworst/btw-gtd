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

        public void DisplayProject(ProjectController.ProjectDisplayModel project)
        {
            _actionList.BeginUpdate();

            _projectName.Text = project.Outcome;
            try
            {
                _actionList.Items.Clear();
                foreach (var action in project.Actions)
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
            public readonly ProjectController.ActionDisplayModel Model;

            public ActionDisplay(ProjectController.ActionDisplayModel model)
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
