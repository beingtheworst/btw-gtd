namespace Gtd.Client
{
    public sealed class MainFormController : 
        IHandle<AppInit>,
        IHandle<CaptureThoughtClicked>
    {
        readonly MainForm _mainForm;
        readonly IPublisher _queue;

        public MainFormController(MainForm mainForm, IPublisher queue)
        {
            _mainForm = mainForm;
            _queue = queue;

            
        }

        public void SubscribeTo(ISubscriber bus)
        {
            bus.Subscribe<AppInit>(this);
            bus.Subscribe<CaptureThoughtClicked>(this);
        }

        public void Handle(AppInit message)
        {
            
        }

        public void Handle(CaptureThoughtClicked message)
        {
            _mainForm.Sync(() =>
                {
                    var c = CaptureThoughtForm.TryGetUserInput(_mainForm);
                    if (null != c) _queue.Publish(new RequestCaptureThought(c));
                });
            
        }
    }
}