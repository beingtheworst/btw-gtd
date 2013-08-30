using Cirrious.MvvmCross.Plugins.Sqlite;

namespace Gtd.Client.Core.Models
{
    public class Project
    {
        // SQLite Error Don't know about Gtd.TrustedSystemId, Switch to simple for now
        //public TrustedSystemId TrustedSystemId { get; set; }
        //public Project ProjectId { get; set; }

        //[PrimaryKey, AutoIncrement]
        //public int Id { get; set; }

        [PrimaryKey]
        public string ProjectId { get; set; }
        public string TrustedSystemId { get; set; }
        public string Outcome { get; set; }
    }
}
