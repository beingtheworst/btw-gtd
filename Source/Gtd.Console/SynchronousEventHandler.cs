using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;

namespace Gtd.Console
{
    public sealed class SynchronousEventHandler
    {
        readonly IList<object> _eventHandlers = new List<object>();
        public void Handle(Event @event)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                // try to execute each Event that happend against
                // each eventHandler in the list and let it handle the Event if it cares or ignore it if it doesn't
                try
                {
                    ((dynamic)eventHandler).When((dynamic)@event);
                }
                catch (RuntimeBinderException e)
                {
                    // binding failure. Ignore
                }
            }
        }
        public void RegisterHandler(object projection)
        {
            _eventHandlers.Add(projection);
        }
    }
}