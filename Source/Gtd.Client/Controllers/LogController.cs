namespace Gtd.Client.Controllers
{
    public interface ILogView
    {
        void Log(string toString);
    }

    sealed class LogController : IHandle<Message>
    {
        readonly ILogView _view;

        public static void Wire(ILogView view, ISubscriber bus)
        {
            var adapter = new LogController(view);

            // have our logger subscribe to the "main event loop"
            // of the application so it can see all Messages (Actions/Events, etc.)
            // the "bus" is part of our in-memory UI messaging architecture (SEDA) 
            bus.Subscribe(adapter);
        }


        LogController(ILogView view)
        {
            _view = view;
        }

        public void Handle(Message message)
        {
            var s = message.ToString();
            var type = message.GetType();
            if (s == type.ToString())
            {
                s = type.FullName.Remove(0, type.Namespace.Length + 1);
            }
            _view.Log(s);
        }
    }

}