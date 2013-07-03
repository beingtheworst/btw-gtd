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

            try
            {
                _actionList.Items.Clear();
                foreach (var action in project.Actions)
                {
                    _actionList.Items.Add(action.Outcome, action.Completed);
                }
            }
            finally
            {
                _actionList.EndUpdate();
            }
        }
    }

    public sealed class ProjectAdapter : IHandle<RequestShowProject>
    {
        readonly ProjectView _control;
        readonly ISystemView _view;
        readonly Region _mainRegion;

        ProjectAdapter(ProjectView control, ISystemView view, Region mainRegion)
        {
            _control = control;
            _view = view;
            _mainRegion = mainRegion;
        }

        public static void Wire(Region mainRegion, IPublisher queuedHandler, ISubscriber bus, ISystemView view)
        {
            // passed from external wire as interface implementor
            var control = new ProjectView();


            var adapter = new ProjectAdapter(control, view, mainRegion);

            


            mainRegion.RegisterDock(control, "project");

            bus.Subscribe<RequestShowProject>(adapter);



        }

        public void Handle(RequestShowProject message)
        {
            
            var project = _view.GetProjectOrNull(message.Id);
            
            _control.Sync(() => _control.DisplayProject(project));
            _mainRegion.SwitchTo("project");
        }



        public sealed class ProjectDisplayModel
        {
            
        }
    }
}
