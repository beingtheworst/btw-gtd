using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Sqlite;

namespace Gtd.Client.Core.Models
{
    public class Action
    {
        // SQLite Error Don't know about Gtd.TrustedSystemId, Switch to simple for now
        //public TrustedSystemId TrustedSystemId { get; set; }
        //public ActionId ActionId { get; set; }
        //public ProjectId ProjectId { get; set; }

        //[PrimaryKey, AutoIncrement]
        //public int Id { get; set; }
        [PrimaryKey]
        public string ActionId { get; set; }
        public string TrustedSystemId { get; set; }
        public string ProjectId { get; set; }
        public string Outcome { get; set; }
    }
}
