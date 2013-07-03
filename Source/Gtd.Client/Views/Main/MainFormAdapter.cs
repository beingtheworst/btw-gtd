using System;

namespace Gtd.Client
{
    public sealed class MainFormAdapter : 
        IHandle<AppInit>,
        IHandle<CaptureThoughtClicked>
    {
        readonly MainForm _mainForm;
        readonly IPublisher _queue;

        public MainFormAdapter(MainForm mainForm, IPublisher queue)
        {
            _mainForm = mainForm;
            _queue = queue;
        }

        public static MainFormAdapter Wire(MainForm form, IPublisher queue, ISubscriber bus)
        {
            var adapter = new MainFormAdapter(form, queue);

            bus.Subscribe<AppInit>(adapter);
            bus.Subscribe<CaptureThoughtClicked>(adapter);
            bus.Subscribe<Message>(new LogSink(message => form.BeginInvoke(new Action(() => form.Log(message.ToString())))));
            form.SetAdapter(adapter);
            
            return adapter;
        }

        sealed class LogSink : IHandle<Message>
        {
            Action<Message> _action;

            public LogSink(Action<Message> action)
            {
                _action = action;
            }

            public void Handle(Message message)
            {
                _action(message);
            }
        }

        public void Publish(Message m)
        {
            _queue.Publish(m);
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