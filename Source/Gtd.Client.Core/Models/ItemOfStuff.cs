using System;

namespace Gtd.Client.Core.Models
{
    public class ItemOfStuff
    {
        public TrustedSystemId Id { get; set; }
        public StuffId StuffId { get; set; }
        public string StuffDescription { get; set; }
        public DateTime TimeUtc { get; set; }
    }
}

