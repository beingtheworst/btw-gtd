namespace Gtd.Client.Views.CaptureThought
{
    public sealed class CaptureThoughtController : IHandle<Ui.CaptureThoughtClicked>
    {
        readonly CaptureThoughtForm _form;
        readonly IMessageQueue _queue;

        CaptureThoughtController(CaptureThoughtForm form, IMessageQueue queue)
        {
            _form = form;
            _queue = queue;
        }

        public static void Wire(CaptureThoughtForm form, ISubscriber source, IMessageQueue target)
        {
            var controller = new CaptureThoughtController(form, target);
            source.Subscribe<Ui.CaptureThoughtClicked>(controller);
        }

        public void Handle(Ui.CaptureThoughtClicked message)
        {
            _form.TryGetUserInput(s => _queue.Enqueue(new Ui.CaptureThoughtWizardCompleted(s)));
        }
    }
}