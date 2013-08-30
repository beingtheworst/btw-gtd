using System.Collections.Generic;
using Cirrious.MvvmCross.Plugins.Messenger;
using Gtd.Client.Core.DataStore;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Inbox
{
    // We use this service so we can have singletons holding all
    // the stuff in the inbox in memory so that the ViewModels are not
    // constantly hitting the database layer to get it

    // probably going to be a Lazy Singleton per config convention in App.cs "Service"
    public class InboxService : IInboxService
    {
        private readonly IInboxRepository _inboxRepository;
        private readonly IMvxMessenger _mvxMessenger;

        public InboxService(IInboxRepository inboxRepository, 
                            IMvxMessenger mvxMessenger)
        {
            // this wont get initialized from the Mvx IoC
            // so we do it here
            _inboxRepository = inboxRepository;
            _mvxMessenger = mvxMessenger;
        }

        // TODO: May want to do validation in here instead of ViewModel
        public void AddStuffToInbox(ItemOfStuff itemOfStuff)
        {
            _inboxRepository.AddStuffToInbox(itemOfStuff);

            // send msg to tell others there was stuff added
            // this can help properties in ViewModels stay updated

            _mvxMessenger.Publish(new InboxChangedMessage(this));
        }

        public IList<ItemOfStuff> AllStuffInInbox()
        {
            // this will likely get cached down the road but use All for now
            return _inboxRepository.AllStuffInInbox();
        }

        public ItemOfStuff Get(string stuffId)
        {
            return _inboxRepository.GetByStuffId(stuffId);
        }

        public void TrashStuff(ItemOfStuff itemOfStuff)
        {
            _inboxRepository.TrashStuff(itemOfStuff);

            // just like we publish a message when we add things (see above), we
            // may also want to do that when we delete things
            _mvxMessenger.Publish(new InboxChangedMessage(this));
        }
    }
}
