using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public interface IInboxRepository
    {
        // assumes dbase will never be HUGE so no need to filter items
        IList<ItemOfStuff> AllStuffInInbox();
        ItemOfStuff GetByStuffId(StuffId stuffId);
        void AddStuffToInbox(ItemOfStuff itemOfStuff);
        void TrashStuff(ItemOfStuff itemOfStuff);
    }
}