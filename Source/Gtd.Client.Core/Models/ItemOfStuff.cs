using System;
using Cirrious.MvvmCross.Plugins.Sqlite;

namespace Gtd.Client.Core.Models
{
    public class ItemOfStuff
    {
        // SQLite Error Don't know about Gtd.TrustedSystemId, Switch to simple for now
        //public TrustedSystemId TrustedSystemId { get; set; }
        //public StuffId StuffId { get; set; }

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string TrustedSystemId { get; set; }
        public string StuffId { get; set; }
        public string StuffDescription { get; set; }
        public DateTime TimeUtc { get; set; }
    }
}

