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

    sealed class LogAdapter : ILogger
    {
        ILogControl _control;
        public LogAdapter(ILogControl control)
        {
            _control = control;
        }

        public void Fatal(string text)
        {
                
        }

        public void Error(string text)
        {
        }

        public void Info(string text)
        {
        }

        public void Debug(string text)
        {
        }

        public void Trace(string text)
        {
        }

        public void Fatal(string format, params object[] args)
        {
        }

        public void Error(string format, params object[] args)
        {
        }

        public void Info(string format, params object[] args)
        {
        }

        public void Debug(string format, params object[] args)
        {
        }

        public void Trace(string format, params object[] args)
        {
        }

        public void FatalException(Exception exc, string text)
        {
        }

        public void ErrorException(Exception exc, string text)
        {
        }

        public void InfoException(Exception exc, string text)
        {
        }

        public void DebugException(Exception exc, string text)
        {
        }

        public void TraceException(Exception exc, string text)
        {
        }

        public void FatalException(Exception exc, string format, params object[] args)
        {
        }

        public void ErrorException(Exception exc, string format, params object[] args)
        {
            _control.Log(String.Format(format, args));
            _control.Log(exc.ToString());
        }

        public void InfoException(Exception exc, string format, params object[] args)
        {
        }

        public void DebugException(Exception exc, string format, params object[] args)
        {
        }

        public void TraceException(Exception exc, string format, params object[] args)
        {
        }
    }
}