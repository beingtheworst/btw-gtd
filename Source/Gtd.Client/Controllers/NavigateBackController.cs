using System;
using System.Collections.Generic;

namespace Gtd.Client
{

    public interface INavigationMenu
    {
        void ToggleNavigateBackButton(bool enabled);
        void SubscribeToNavigateBack(Action handler);
    }

    public sealed class NavigateBackController : 
        IHandle<UI.NavigateCommand>
    {
        readonly INavigationMenu _menu;
        readonly IMessageQueue _output;

        readonly Stack<UI.NavigateCommand> _stack = new Stack<UI.NavigateCommand>();
       

        NavigateBackController(INavigationMenu menu, IMessageQueue output)
        {
            _menu = menu;
            _output = output;
        }

        public static void Wire(ISubscriber input, IMessageQueue output, INavigationMenu menu)
        {
            var adapter = new NavigateBackController(menu, output);
            input.Subscribe<UI.NavigateCommand>(adapter);
            //input.Subscribe<UI.NavigateBackClicked>(adapter);
            menu.ToggleNavigateBackButton(false);
            menu.SubscribeToNavigateBack(adapter.GoBack);
        }

        public  void GoBack()
        {

            var prev = _stack.Pop();
            _output.Enqueue(_stack.Pop());
            _menu.ToggleNavigateBackButton(_stack.Count > 1);
        }

        public void Handle(UI.NavigateCommand message)
        {
            // ignore same instance of the message
            if (_stack.Count > 0 && _stack.Peek() == message)
                return;

            _stack.Push(message);

            _menu.ToggleNavigateBackButton(_stack.Count>1);
        }
    }
}