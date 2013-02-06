using System;
using System.Collections.Generic;

namespace Gtd.Shell.Projections
{
    public sealed class InboxView
    {
        public IDictionary<TenantId, List<InboxEntry>> TenantInboxes = new Dictionary<TenantId, List<InboxEntry>>();

        public sealed class InboxEntry
        {
            public Guid ItemId;
            public string Thought;
            public DateTime Added;
        }
    }
    public sealed class InboxProjection
    {
        public InboxView ViewInstance = new InboxView();
        
        public void When(TenantCreated e)
        {
            ViewInstance.TenantInboxes.Add(e.Id, new List<InboxView.InboxEntry>());
        }

        public void When(InboxEntryCaptured c)
        {
            ViewInstance.TenantInboxes[c.Id].Add(new InboxView.InboxEntry()
                {
                    ItemId = c.RequestId,
                    Thought = c.Name,
                    Added = c.TimeUtc
                });
        }
    }
}