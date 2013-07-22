namespace Gtd.Client.Views.CaptureThought
{
    public sealed class DefineProjectController : IHandle<UI.DefineProjectClicked>
    {
        readonly DefineProjectForm _form;
        readonly IMessageQueue _queue;

        DefineProjectController(DefineProjectForm form, IMessageQueue queue)
        {
            _form = form;
            _queue = queue;
        }

        public static void Wire(DefineProjectForm form, ISubscriber source, IMessageQueue target)
        {
            var controller = new DefineProjectController(form, target);
            source.Subscribe<UI.DefineProjectClicked>(controller);
        }

        public void Handle(UI.DefineProjectClicked message)
        {
            _form.TryGetUserInput(input => _queue.Enqueue(new UI.DefineNewProjectWizardCompleted(input)));
        }
    }
}