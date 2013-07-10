using System.Windows.Forms;
using Gtd.Client.Models;

namespace Gtd.Client.Views.Actions
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        public void DisplayProject(Models.ProjectView project)
        {
            _actionList.BeginUpdate();

            _projectName.Text = project.GetTitle();
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
            public readonly ActionView View;

            public ActionDisplay(ActionView view)
            {
                View = view;
            }

            public override string ToString()
            {
                return View.Outcome;
            }
        }

        public void AttachTo(ProjectController controller)
        {
            _actionList.ItemCheck += (sender, args) =>
                {
                    var display = (ActionDisplay) _actionList.Items[args.Index];
                    if (args.NewValue == CheckState.Checked)
                    {
                        controller.RequestActionCheck(display.View.Id);
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
