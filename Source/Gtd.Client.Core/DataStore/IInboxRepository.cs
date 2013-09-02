using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public interface IInboxRepository
    {
        // assumes dbase will never be HUGE so no need to filter items

        // SQLite doesn't like my custom Id types. Goign to string for now.

        IList<ItemOfStuff> AllStuffInInbox();
        //ItemOfStuff GetByStuffId(StuffId stuffId);
        ItemOfStuff GetByStuffId(string stuffId);
        void AddStuffToInbox(ItemOfStuff itemOfStuff);
        void TrashStuff(ItemOfStuff itemOfStuff);
    }
}