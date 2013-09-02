using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Inbox
{
    public interface IInboxService
    {
        IList<ItemOfStuff> AllStuffInInbox();
        ItemOfStuff GetByStuffId(string stuffId);
        void AddStuffToInbox(ItemOfStuff itemOfStuff);
        void TrashStuff(ItemOfStuff itemOfStuff);
    }
}
