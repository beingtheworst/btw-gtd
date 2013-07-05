using System;

namespace Gtd.Client
{
    public sealed class PanelHideController : 
        IHandle<Ui.InboxDisplayed>,
        IHandle<Ui.ProjectDisplayed>
    {
        readonly IPublisher _queue;

        public static void WireTo(ISubscriber bus, IPublisher queue)
        {
            var controller = new PanelHideController(queue);

            bus.Subscribe<Ui.InboxDisplayed>(controller);
            bus.Subscribe<Ui.ProjectDisplayed>(controller);
        }

        Action _onHide = () => { };

        

        PanelHideController(IPublisher queue)
        {
            _queue = queue;
        }

        public void Handle(Ui.InboxDisplayed message)
        {
            _onHide();
            _onHide = () => _queue.Publish(new Ui.InboxHidden());
        }

        public void Handle(Ui.ProjectDisplayed message)
        {
            _onHide();
            _onHide = () => _queue.Publish(new Ui.ProjectHidden(message.Id));
        }
    }
}