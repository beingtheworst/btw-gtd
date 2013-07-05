using System.Windows.Forms;

namespace Gtd.Client.Views.Actions
{
    public partial class ProjectView : UserControl
    {
        public ProjectView()
        {
            InitializeComponent();
        }

        public void DisplayProject(Client.ProjectView project)
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

        public void AttachTo(ProjectAdapter adapter)
        {
            _actionList.ItemCheck += (sender, args) =>
                {
                    var display = (ActionDisplay) _actionList.Items[args.Index];
                    if (args.NewValue == CheckState.Checked)
                    {
                        adapter.RequestActionCheck(display.View.Id);
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

    public sealed class ProjectAdapter : IHandle<Ui.DisplayProject>
    {
        readonly ProjectView _control;
        readonly ISystemView _view;
        readonly Region _mainRegion;
        readonly IPublisher _sync;

        ProjectAdapter(ProjectView control, ISystemView view, Region mainRegion, IPublisher sync)
        {
            _control = control;
            _view = view;
            _mainRegion = mainRegion;
            _sync = sync;
        }

        public static void Wire(Region mainRegion, IPublisher queuedHandler, ISubscriber bus, ISystemView view)
        {
            // passed from external wire as interface implementor
            var control = new ProjectView();
            

            var adapter = new ProjectAdapter(control, view, mainRegion, queuedHandler);


            control.AttachTo(adapter);

            mainRegion.RegisterDock(control, "project");

            bus.Subscribe<Ui.DisplayProject>(adapter);
        }

        public void Handle(Ui.DisplayProject message)
        {
            
            var project = _view.GetProjectOrNull(message.Id);
            
            _control.Sync(() => _control.DisplayProject(project));
            _mainRegion.SwitchTo("project");
        }



        public sealed class ProjectDisplayModel
        {
            
        }

        public void RequestActionCheck(ActionId id)
        {
            _sync.Publish(new Ui.CompleteAction(id));
        }
    }

}
