using System.Windows.Forms;
using Gtd.Client.Models;

namespace Gtd.Client.Views.Project
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
            _grid.DataSource = _source;
        }

        readonly BindingSource _source = new BindingSource();


        ProjectId _project;
        public void DisplayProject(FilteredProject project)
        {
            _project = project.Info.ProjectId;
            _projectName.Text = string.Format("{0} ({1})", project.Info.Outcome, project.ActionCount);

            // TODO: smarter update for the case when we remove item
            if (_source.Count == project.FilteredActions.Count)
            {
                for (int i = 0; i < project.FilteredActions.Count; i++)
                {
                    _source[i] = new ActionDisplay(project.FilteredActions[i],_controller);
                }
                return;
            }

            _source.Clear();
            foreach (var action in project.FilteredActions)
            {
                _source.Add(new ActionDisplay(action,_controller));
            }
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

            _addAction.Click += (sender, args) => _controller.RequestAddAction(_project);
        }
    }
}
