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

            // have our logger subscribe to the "main event loop"
            // of the application so it can see all Messages (Actions/Events, etc.)
            // the "bus" is part of our in-memory UI messaging architecture (SEDA) 
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