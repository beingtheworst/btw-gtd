using Gtd.Client.Models;

namespace Gtd.Client.Views.Actions
{
    public sealed class ProjectController : IHandle<Ui.DisplayProject>
    {
        readonly ProjectView _control;
        readonly ClientContext _view;
        readonly Region _mainRegion;
        readonly IPublisher _bus;

        ProjectController(ProjectView control, ClientContext view, Region mainRegion, IPublisher bus)
        {
            _control = control;
            _view = view;
            _mainRegion = mainRegion;
            _bus = bus;
        }

        public static void Wire(Region mainRegion, IPublisher queuedHandler, ISubscriber bus, ClientContext view)
        {
            // passed from external wire as interface implementor
            var control = new ProjectView();
            

            var adapter = new ProjectController(control, view, mainRegion, queuedHandler);


            control.AttachTo(adapter);

            mainRegion.RegisterDock(control, "project");

            bus.Subscribe(adapter);
        }

        public void Handle(Ui.DisplayProject message)
        {
            var project = _view.GetProjectOrNull(message.Id);
            
            _control.Sync(() => _control.DisplayProject(project));
            _mainRegion.SwitchTo("project");
            _bus.Publish(new Ui.ProjectDisplayed(message.Id));
        }



        public sealed class ProjectDisplayModel
        {
            
        }

        public void RequestActionCheck(ActionId id)
        {
            _bus.Publish(new Ui.CompleteActionClicked(id));
        }
    }
}