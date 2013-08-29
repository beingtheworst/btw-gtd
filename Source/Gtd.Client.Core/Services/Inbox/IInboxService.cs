using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Inbox
{
    public interface IInboxService
    {
        IList<ItemOfStuff> AllStuffInInbox();
        ItemOfStuff Get(StuffId stuffId);
        void AddStuffToInbox(ItemOfStuff stuffId);
        void TrashStuff(ItemOfStuff stuffId);
    }
}
