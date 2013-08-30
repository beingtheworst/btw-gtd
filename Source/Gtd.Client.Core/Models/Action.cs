using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gtd.Client.Core.Models
{
    public class Action
    {
        public TrustedSystemId TrustedSystemId { get; set; }
        public ActionId ActionId { get; set; }
        public ProjectId ProjectId { get; set; }
        public string Outcome { get; set; }
    }
}
