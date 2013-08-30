using System.Collections.Generic;
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
        private readonly InboxRepository _inboxRepository;

        public InboxService(InboxRepository inboxRepository)
        {
            // this wont get initialized from the Mvx IoC
            // so we do it here
            _inboxRepository = inboxRepository;

        }

        // TODO: May want to do validation in here instead of ViewModel
        public void AddStuffToInbox(ItemOfStuff itemOfStuff)
        {
            _inboxRepository.AddStuffToInbox(itemOfStuff);

            // TODO: TBD if we need/want this
            // send msg to tell others there was an Item added
            // this can help properties in ViewModels stay updated

            

            // _messenger.Publish(new StuffAddedToInboxMessage(this));
        }

        public IList<ItemOfStuff> AllStuffInInbox()
        {
            // this will likely get cached down the road but use All for now
            return _inboxRepository.AllStuffInInbox();
        }

        public ItemOfStuff Get(StuffId stuffId)
        {
            return _inboxRepository.GetByStuffId(stuffId);
        }

        public void TrashStuff(ItemOfStuff itemOfStuff)
        {
            _inboxRepository.TrashStuff(itemOfStuff);

            // just like we publish a message when we add things (see above), we
            // may also want to do that when we delete things
            // _messenger.Publish(new TbdMessage(this));
        }
    }
}
