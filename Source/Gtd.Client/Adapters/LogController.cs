namespace Gtd.Client
{
    public interface ILogControl
    {
        void Log(string toString);
    }

    sealed class LogController : IHandle<Message>
    {
        readonly ILogControl _control;

        public static void Wire(ILogControl control, ISubscriber bus)
        {
            var adapter = new LogController(control);
            bus.Subscribe<Message>(adapter);
        }


        LogController(ILogControl control)
        {
            _control = control;
        }

        public void Handle(Message message)
        {
            _control.Log(message.ToString());
        }
    }
}