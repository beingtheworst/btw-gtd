namespace Gtd.Client.Views.CaptureThought
{
    public sealed class DefineProjectController : IHandle<Ui.DefineProjectClicked>
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
            source.Subscribe<Ui.DefineProjectClicked>(controller);
        }

        public void Handle(Ui.DefineProjectClicked message)
        {
            _form.TryGetUserInput(s => _queue.Enqueue(new Ui.DefineNewProject(s)));
        }
    }
}