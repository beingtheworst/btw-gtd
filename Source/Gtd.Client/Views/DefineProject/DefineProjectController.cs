using System;

namespace Gtd.Client.Views.CaptureThought
{

    public interface IDefineProjectWizard
    {
        void TryGetUserInput(Action<string> callbackWithOutcome);
    }
    public sealed class DefineProjectController : IHandle<UI.DefineProjectClicked>
    {
        readonly IDefineProjectWizard _form;
        readonly IMessageQueue _queue;

        DefineProjectController(IDefineProjectWizard form, IMessageQueue queue)
        {
            _form = form;
            _queue = queue;
        }

        public static void Wire(IDefineProjectWizard form, ISubscriber source, IMessageQueue target)
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