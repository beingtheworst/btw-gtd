using System;

namespace Gtd.Client
{
    public sealed class MainFormController : 
        IHandle<AppInit>,
        IHandle<CaptureThoughtClicked>,
        IHandle<Ui.InboxDisplayed>, IHandle<Ui.ProjectDisplayed>
    {
        readonly MainForm _mainForm;
        readonly IPublisher _queue;
        readonly FilterService _service;

        public MainFormController(MainForm mainForm, IPublisher queue, FilterService service)
        {
            _mainForm = mainForm;
            _queue = queue;
            _service = service;
        }

        public static MainFormController Wire(MainForm form, IPublisher queue, ISubscriber bus, FilterService service)
        {
            var adapter = new MainFormController(form, queue, service);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<CaptureThoughtClicked>(adapter);
            bus.Subscribe<Ui.InboxDisplayed>(adapter);
            bus.Subscribe<Ui.ProjectDisplayed>(adapter);

            form.SetAdapter(adapter);
            
            return adapter;
        }


        public void Publish(Message m)
        {
            _queue.Publish(m);
        }

        

        public void Handle(AppInit message)
        {
            _mainForm.DisplayFilters(_service.Filters);
        }

        public void Handle(CaptureThoughtClicked message)
        {
            _mainForm.Sync(() =>
                {
                    var c = CaptureThoughtForm.TryGetUserInput(_mainForm);
                    if (null != c) _queue.Publish(new Ui.CaptureThought(c));
                });
            
        }


        public void Handle(Ui.InboxDisplayed message)
        {
            // replace with panel-specific menu may be
            _mainForm.ShowInboxMenu();
        }

        public void Handle(Ui.ProjectDisplayed message)
        {
            _mainForm.ShowProjectMenu(message.Id);
        }
    }
}