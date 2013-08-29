using System;

namespace Gtd.Client.Core.Models
{
    // Our Class to Hold the Collected data Item to put in SQLite Database
    public class ItemOfStuff
    {
        // TODO: Can't reference non-PCL DLL's in a PCL so reusing
        // TODO: So, see what happens if I try to convert to PCL just for the heck of it.

        //[DataMember(Order = 1)]
        //public TrustedSystemId Id { get; private set; }
        //[DataMember(Order = 2)]
        //public StuffId StuffId { get; private set; }
        //[DataMember(Order = 3)]
        //public string StuffDescription { get; private set; }
        //[DataMember(Order = 4)]
        //public DateTime TimeUtc { get; private set; }


        //public int Id { get; set; }

        //public string Caption { get;  set; }
        //public string Notes { get; set; }

        //// we will at least know that when we stored, it was Utc
        //// it may come out of dbase converted odd on various platforms tho
        //public DateTime WhenUtc { get; set; }

        //public bool LocationKnown { get; set; }
        //public double Lat { get; set; }
        //public double Lng { get; set; }

        //public string ImagePath { get; set; }
    }
}

