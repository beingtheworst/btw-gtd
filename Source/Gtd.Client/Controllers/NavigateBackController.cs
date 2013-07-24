using System;
using System.Collections.Generic;

namespace Gtd.Client.Controllers
{

    public interface INavigateBackView
    {
        void EnableNavigateBackButton(bool enabled);
        void SubscribeToNavigateBack(Action handler);
    }

    public sealed class NavigateBackController : 
        IHandle<UI.NavigateCommand>
    {
        readonly INavigateBackView _menu;
        readonly IMessageQueue _output;

        readonly Stack<UI.NavigateCommand> _stack = new Stack<UI.NavigateCommand>();
       

        NavigateBackController(INavigateBackView menu, IMessageQueue output)
        {
            _menu = menu;
            _output = output;
        }

        public static void Wire(ISubscriber input, IMessageQueue output, INavigateBackView menu)
        {
            var adapter = new NavigateBackController(menu, output);
            input.Subscribe<UI.NavigateCommand>(adapter);

            menu.EnableNavigateBackButton(false);
            menu.SubscribeToNavigateBack(adapter.GoBack);
        }

        public  void GoBack()
        {
            // we discard current view and go to the history
            _stack.Pop();
            _output.Enqueue(_stack.Pop());
            _menu.EnableNavigateBackButton(_stack.Count > 1);
        }

        public void Handle(UI.NavigateCommand message)
        {
            // ignore same instance of the message
            if (_stack.Count > 0 && _stack.Peek() == message)
                return;

            _stack.Push(message);

            _menu.EnableNavigateBackButton(_stack.Count>1);
        }
    }
}