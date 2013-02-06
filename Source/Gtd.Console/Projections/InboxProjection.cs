using System;
using System.Collections.Generic;

namespace Gtd.Shell.Projections
{
    public sealed class InboxView
    {
        public IDictionary<TenantId, List<Thought>> TenantInboxes = new Dictionary<TenantId, List<Thought>>();

        public sealed class Thought
        {
            public Guid ItemId;
            public string Subject;
            public DateTime Added;
        }
    }
    public sealed class InboxProjection
    {
        public InboxView ViewInstance = new InboxView();
        
        public void When(TenantCreated e)
        {
            ViewInstance.TenantInboxes.Add(e.Id, new List<InboxView.Thought>());
        }

        public void When(ThoughtCaptured evnt)
        {
            ViewInstance.TenantInboxes[evnt.Id].Add(new InboxView.Thought()
                {
                    ItemId = evnt.ThoughtId,
                    Subject = evnt.Thought,
                    Added = evnt.TimeUtc
                });
        }
        public void When(ThoughtArchived e)
        {
            ViewInstance.TenantInboxes[e.Id].RemoveAll(t => t.ItemId == e.ThoughtId);
        }
    }
}