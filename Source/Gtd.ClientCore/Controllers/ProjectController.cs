using System;
using System.Collections.ObjectModel;
using Gtd.Client;
using Gtd.Client.Models;

namespace Gtd.ClientCore.Controllers
{
    public interface IProjectView
    {
        void ShowView(FilteredProject project);

        void SubscribeOutcomeChanged(Action<ActionId,string> e);
        void SubscribeActionCompleted(Action<ActionId> e);
        void SubscribeAddActionClicked(Action<ProjectId> e);
        void SubscribeToDragStart(Action<DragActions> callback);
    }

    public sealed class ProjectController : 
        IHandle<UI.DisplayProject>,
        IHandle<UI.ActionFilterChanged>,
        
        IHandle<Dumb.ActionUpdated>,
        IHandle<Dumb.ActionAdded>,
        IHandle<Dumb.ActionRemoved>
    {
        readonly IProjectView _control;
        readonly ClientPerspective _perspective;

        readonly IMessageQueue _bus;

        ProjectController(IProjectView control, ClientPerspective perspective,  IMessageQueue bus)
        {
            _control = control;
            _perspective = perspective;
            
            _bus = bus;
        }

        public static void Wire(IProjectView view, IMessageQueue queuedHandler, ISubscriber bus, ClientPerspective model)
        {
            var controller = new ProjectController(view, model, queuedHandler);

            bus.Subscribe<UI.ActionFilterChanged>(controller);
            bus.Subscribe<UI.DisplayProject>(controller);
            bus.Subscribe<Dumb.ActionUpdated>(controller);
            bus.Subscribe<Dumb.ActionAdded>(controller);
            bus.Subscribe<Dumb.ActionRemoved>(controller);

            view.SubscribeActionCompleted(controller.CompleteAction);
            view.SubscribeOutcomeChanged(controller.ChangeOutcome);
            view.SubscribeAddActionClicked(controller.RequestAddAction);
            view.SubscribeToDragStart(controller.StartDrag);
        }

        void StartDrag(DragActions obj)
        {
            _bus.Enqueue(new UI.DragStarted(new ActionDragManager(obj.Request,obj.Actions,_bus)));

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
            _bus.Enqueue(new UI.ProjectDisplayed(project.Info));
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

        public void Handle(Dumb.ActionRemoved e)
        {
            if (e.Action.ProjectId.Id != _currentProject.Id) return;

            ReloadView(_currentProject);
        }

        public void CompleteAction(ActionId id)
        {
            _bus.Enqueue(new UI.CompleteActionClicked(id));
        }

        public void Publish(Message message)
        {
            _bus.Enqueue(message);
        }

        public void RequestAddAction(ProjectId id)
        {
            _bus.Enqueue(new UI.AddActionClicked(id));
        }

        public void ChangeOutcome(ActionId id, string outcome)
        {
            _bus.Enqueue(new UI.ChangeActionOutcome(id, outcome));
        }
    }
}