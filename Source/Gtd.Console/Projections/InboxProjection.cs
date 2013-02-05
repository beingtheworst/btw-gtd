using System.Collections.Generic;

namespace Gtd.Shell.Projections
{
    public sealed class InboxProjection
    {
        public sealed class InboxList
        {
            public IList<string> Items = new List<string>(); 
        }

        public IDictionary<TenantId, InboxList> Inbox = new Dictionary<TenantId, InboxList>(); 
        
        public void When(InboxEntryCaptured c)
        {
            Inbox.GetOrAdd(c.Id).Items.Add(c.Name);
        }
    }

    public static class ExtendDictionary
    {
        public static V GetOrAdd<K, V>(this IDictionary<K, V> dict, K key) where V : new ()
        {
            V value;
            if (!dict.TryGetValue(key, out value))
            {
                value = new V();
                dict.Add(key,value);
            }
            return value;
        }
    }
}