using System;
using Gtd.Client.Models;

namespace Gtd.Client.Views.Project
{

    public interface IProjectView
    {

        void ShowView(FilteredProject project);

        void SubscribeOutcomeChanged(Action<ActionId,string> e);
        void SubscribeActionCompleted(Action<ActionId> e);
        void SubscribeAddActionClicked(Action<ProjectId> e);
    }

    public sealed class ProjectController : 
        IHandle<UI.DisplayProject>,
        IHandle<UI.ActionFilterChanged>,
        IHandle<Dumb.ActionUpdated>,
        IHandle<Dumb.ActionAdded>
    {
        readonly IProjectView _control;
        readonly ClientPerspective _perspective;
        
        readonly IPublisher _bus;

        ProjectController(IProjectView control, ClientPerspective perspective,  IPublisher bus)
        {
            _control = control;
            _perspective = perspective;
            
            _bus = bus;
        }

        public static void Wire(IProjectView view, IPublisher queuedHandler, ISubscriber bus, ClientPerspective model)
        {
            var adapter = new ProjectController(view, model, queuedHandler);

            bus.Subscribe<UI.ActionFilterChanged>(adapter);
            bus.Subscribe<UI.DisplayProject>(adapter);
            bus.Subscribe<Dumb.ActionUpdated>(adapter);
            bus.Subscribe<Dumb.ActionAdded>(adapter);

            view.SubscribeActionCompleted(adapter.CompleteAction);
            view.SubscribeOutcomeChanged(adapter.ChangeOutcome);
            view.SubscribeAddActionClicked(adapter.RequestAddAction);
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

            _control.ShowView(project);
            _bus.Publish(new UI.ProjectDisplayed(project.Info));
        }
        
        public void Handle(UI.ActionFilterChanged message)
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

        public void CompleteAction(ActionId id)
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

        public void ChangeOutcome(ActionId id, string outcome)
        {
            _bus.Publish(new UI.ChangeActionOutcome(id, outcome));
        }
    }
}