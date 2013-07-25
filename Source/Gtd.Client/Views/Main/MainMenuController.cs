using System;
using Gtd.Shell.Filters;
using System.Linq;

namespace Gtd.Client
{
    public interface IMainMenu
    {
        void SubscribeToAddStuffClicked(Action callback);
        void SubscribeToDefineProjectClicked(Action callback);
        void SubscribeToGotoInboxClicked(Action callback);

        void SubscribeToSelectedFilterChanged(Action<IFilterCriteria> callback);

        void ShowProjectMenu(ProjectId id);
        void ShowInboxMenu();
    }

    public sealed class MainMenuController : 
        IHandle<UI.InboxDisplayed>, IHandle<UI.ProjectDisplayed>
    {
        readonly IMainMenu _mainForm;
        
        
        public MainMenuController(IMainMenu mainForm)
        {
            _mainForm = mainForm;
            
        }

        public static MainMenuController Wire(IMainMenu form, IMessageQueue queue, ISubscriber bus)
        {
            var adapter = new MainMenuController(form);

            bus.Subscribe<UI.InboxDisplayed>(adapter);
            bus.Subscribe<UI.ProjectDisplayed>(adapter);

            form.SubscribeToAddStuffClicked(() => queue.Enqueue(new UI.AddStuffClicked()));
            form.SubscribeToDefineProjectClicked(() => queue.Enqueue(new UI.DefineProjectClicked()));
            form.SubscribeToGotoInboxClicked(() => queue.Enqueue(new UI.DisplayInbox()));
            form.SubscribeToSelectedFilterChanged(x => queue.Enqueue(new UI.ActionFilterChanged(x)));
            
            
            return adapter;
        }


        
        
        public void Handle(UI.InboxDisplayed message)
        {
            // replace with panel-specific menu maybe
            _mainForm.ShowInboxMenu();
        }

        public void Handle(UI.ProjectDisplayed message)
        {
            _mainForm.ShowProjectMenu(message.Project.ProjectId);
        }
    }
}