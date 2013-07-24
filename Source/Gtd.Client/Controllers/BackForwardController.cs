using System.Collections.Generic;

namespace Gtd.Client
{

    public interface INavigationMenu
    {
        void ToggleBackButton(bool enabled);
    }

    public sealed class BackForwardController : 
        IHandle<UI.NavigateCommand>,
        IHandle<UI.NavigateBackClicked>
    {
        readonly INavigationMenu _menu;
        readonly IMessageQueue _output;

        readonly Stack<UI.NavigateCommand> _stack = new Stack<UI.NavigateCommand>();
       

        BackForwardController(INavigationMenu menu, IMessageQueue output)
        {
            _menu = menu;
            _output = output;
        }

        public static void Wire(ISubscriber input, IMessageQueue output, INavigationMenu menu)
        {
            var adapter = new BackForwardController(menu, output);
            input.Subscribe<UI.NavigateCommand>(adapter);
            input.Subscribe<UI.NavigateBackClicked>(adapter);
            menu.ToggleBackButton(false);
        }

        public void Handle(UI.NavigateCommand message)
        {
            _stack.Push(message);

            _menu.ToggleBackButton(_stack.Count>1);
        }
        
        public void Handle(UI.NavigateBackClicked message)
        {
            // we need to go from here back
            var prev = _stack.Pop();
            _output.Enqueue(_stack.Pop());
            _menu.ToggleBackButton(_stack.Count>1);
        }
    }
}