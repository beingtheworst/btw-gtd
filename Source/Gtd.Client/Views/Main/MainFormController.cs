using Gtd.Shell.Filters;
using System.Linq;

namespace Gtd.Client
{
    public sealed class MainFormController : 
        IHandle<UI.InboxDisplayed>, IHandle<UI.ProjectDisplayed>
    {
        readonly MainForm _mainForm;
        readonly IPublisher _queue;
        
        public MainFormController(MainForm mainForm, IPublisher queue)
        {
            _mainForm = mainForm;
            _queue = queue;
        }

        public static MainFormController Wire(MainForm form, IPublisher queue, ISubscriber bus)
        {
            var adapter = new MainFormController(form, queue);

            bus.Subscribe<UI.InboxDisplayed>(adapter);
            bus.Subscribe<UI.ProjectDisplayed>(adapter);

            form.SetAdapter(adapter);
            
            return adapter;
        }


        public void Publish(Message m)
        {
            _queue.Publish(m);
        }


        // These simple Handle methods are an example of how 
        // breaking the UI down into event-driven standalone controllers
        // allows us to make dynamic menu changes simply based on UI Events
        // that have occured.  If InBoxDisplayed, show this.
        // If ProjectDisplayed, show that.

        
        public void Handle(UI.InboxDisplayed message)
        {
            // replace with panel-specific menu maybe
            _mainForm.ShowInboxMenu();
        }

        public void Handle(UI.ProjectDisplayed message)
        {
            _mainForm.ShowProjectMenu(message.Project.ProjectId);
        }
    }
}