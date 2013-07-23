using System;
using Gtd.Shell;

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
            bus.Subscribe(adapter);
        }


        LogController(ILogControl control)
        {
            _control = control;
        }

        public void Handle(Message message)
        {
            var s = message.ToString();
            var type = message.GetType();
            if (s == type.ToString())
            {
                s = type.FullName.Remove(0, type.Namespace.Length + 1);
            }
            _control.Log(s);
        }
    }

}