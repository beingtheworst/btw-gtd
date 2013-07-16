using System.Collections.ObjectModel;
using Gtd.Client.Models;
using System.Linq;

namespace Gtd.Client.Views.Project
{
    public sealed class ProjectController : 
        IHandle<UI.DisplayProject>,
        IHandle<UI.FilterChanged>,
        IHandle<Dumb.ActionUpdated>
    {
        readonly ProjectView _control;
        readonly ClientPerspective _view;
        readonly Region _mainRegion;
        readonly IPublisher _bus;

        ProjectController(ProjectView control, ClientPerspective view, Region mainRegion, IPublisher bus)
        {
            _control = control;
            _view = view;
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
        }

        ProjectId _currentProject = ProjectId.Empty;

        public void Handle(UI.DisplayProject message)
        {
            _currentProject = message.Id;
            ReloadView(message.Id);
        }

        void ReloadView(ProjectId projectId)
        {
            var project = _view.GetProjectOrNull(projectId);
            var filter = _view.CurrentFilter;

            var actions =
                filter.FilterActions(project).Select(a => new ActionDisplayModel(a.Id, a.Outcome, a.Completed)).ToList();
            var count = filter.FormatActionCount(actions.Count);

            var display = new ProjectDisplayModel(project.Outcome, count, actions.AsReadOnly(), project.ProjectId);


            _control.Sync(() => _control.DisplayProject(display));
            _mainRegion.SwitchTo("project");
            _bus.Publish(new UI.ProjectDisplayed(projectId));
        }


        public sealed class ProjectDisplayModel
        {
            public readonly string Outcome;
            public readonly string Count;

            public readonly ReadOnlyCollection<ActionDisplayModel> Actions;

            public readonly ProjectId Id;

            public ProjectDisplayModel(string outcome, string count, ReadOnlyCollection<ActionDisplayModel> actions, ProjectId id)
            {
                Outcome = outcome;
                Count = count;
                Actions = actions;
                Id = id;
            }
        }

        public sealed class ActionDisplayModel
        {
            public readonly ActionId ActionId;
            public readonly string Outcome;
            public readonly bool Completed;

            public ActionDisplayModel(ActionId actionId, string outcome, bool completed)
            {
                ActionId = actionId;
                Outcome = outcome;
                Completed = completed;
            }
        }

        


        public void RequestActionCheck(ActionId id)
        {
            _bus.Publish(new UI.CompleteActionClicked(id));
        }

        public void Handle(UI.FilterChanged message)
        {
            if (!_currentProject.IsEmpty)
            ReloadView(_currentProject);
        }

        public void Handle(Dumb.ActionUpdated message)
        {
            if (message.ProjectId.Id == _currentProject.Id)
                ReloadView(_currentProject);
        }
    }
}