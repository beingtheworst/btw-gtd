using System;
using System.Collections.Generic;

namespace Gtd.Shell.Projections
{
    public sealed class InboxView
    {
        public IDictionary<TrustedSystemId, List<Thought>> TrustedSystemInboxes = new Dictionary<TrustedSystemId, List<Thought>>();

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

        public void When(TrustedSystemCreated evnt)
        {
            ViewInstance.TrustedSystemInboxes.Add(evnt.Id, new List<InboxView.Thought>());
        }

        public void When(ThoughtCaptured evnt)
        {
            ViewInstance.TrustedSystemInboxes[evnt.Id].Add(new InboxView.Thought()
                {
                    ItemId = evnt.ThoughtId,
                    Subject = evnt.Thought,
                    Added = evnt.TimeUtc
                });
        }
        public void When(ThoughtArchived evnt)
        {
            ViewInstance.TrustedSystemInboxes[evnt.Id].RemoveAll(t => t.ItemId == evnt.ThoughtId);
        }
    }
}