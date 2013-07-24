using Gtd.Client.Models;

namespace Gtd.Client.Views.Project
{
    public sealed class ProjectController : 
        IHandle<UI.DisplayProject>,
        IHandle<UI.FilterChanged>,
        IHandle<Dumb.ActionUpdated>,
        IHandle<Dumb.ActionAdded>
    {
        readonly ProjectView _control;
        readonly ClientPerspective _perspective;
        readonly Region _mainRegion;
        readonly IPublisher _bus;

        ProjectController(ProjectView control, ClientPerspective perspective, Region mainRegion, IPublisher bus)
        {
            _control = control;
            _perspective = perspective;
            _mainRegion = mainRegion;
            _bus = bus;
        }

        public static void Wire(Region mainRegion, IPublisher queuedHandler, ISubscriber bus, ClientPerspective view)
        {
            // passed from external wire as interface implementor
            var control = new ProjectView();
            

            var adapter = new ProjectController(control, view, mainRegion, queuedHandler);


            control.AttachTo(adapter);

            mainRegion.RegisterDock(control, "project");

            bus.Subscribe<UI.FilterChanged>(adapter);
            bus.Subscribe<UI.DisplayProject>(adapter);
            bus.Subscribe<Dumb.ActionUpdated>(adapter);
            bus.Subscribe<Dumb.ActionAdded>(adapter);
        }

        ProjectId _currentProject = ProjectId.Empty;

        public void Handle(UI.DisplayProject message)
        {
            _currentProject = message.Id;
            ReloadView(message.Id);
        }

        void ReloadView(ProjectId projectId)
        {
            // TODO: smart update
            var project = _perspective.GetProject(projectId);

            _control.Sync(() => _control.DisplayProject(project));
            _mainRegion.SwitchTo("project");
            _bus.Publish(new UI.ProjectDisplayed(project.Info));
        }
        
        public void Handle(UI.FilterChanged message)
        {
            if (!_currentProject.IsEmpty)
            ReloadView(_currentProject);
        }

        public void Handle(Dumb.ActionUpdated message)
        {
            if (message.Action.ProjectId.Id != _currentProject.Id) return;
            ReloadView(_currentProject);
        }

        public void Handle(Dumb.ActionAdded message)
        {
            if (message.Action.ProjectId.Id != _currentProject.Id) return;

            ReloadView(_currentProject);
        }

        public void RequestActionCheck(ActionId id)
        {
            _bus.Publish(new UI.CompleteActionClicked(id));
        }

        public void Publish(Message message)
        {
            _bus.Publish(message);
        }

        public void RequestAddAction(ProjectId id)
        {
            _bus.Publish(new UI.AddActionClicked(id));
        }

        public void RequestOutcomeChange(ActionId id, string outcome)
        {
            _bus.Publish(new UI.ChangeActionOutcome(id, outcome));
        }
    }
}