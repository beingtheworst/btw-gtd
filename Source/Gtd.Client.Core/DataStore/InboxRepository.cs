using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.Plugins.Sqlite;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public class InboxRepository : IInboxRepository
    {
        // TODO: wrap this in a using instead/later
        private readonly ISQLiteConnection _sqlConnection;

        public InboxRepository(ISQLiteConnectionFactory sqlConnectionFactory)
        {
            // use factory to get the connection to SQLite database
            // pass create the name of the SQLite database
            _sqlConnection = sqlConnectionFactory.Create("btwgtd.sqlite");

            // Create or ensure the dbase Table is created for our ItemOfStuff
            _sqlConnection.CreateTable<ItemOfStuff>();
        }

        public void AddStuffToInbox(ItemOfStuff itemOfStuff)
        {
            _sqlConnection.Insert(itemOfStuff);
        }

        public IList<ItemOfStuff> AllStuffInInbox()
        {
            return _sqlConnection
                .Table<ItemOfStuff>()
                .OrderBy(item => item.TimeUtc)
                .ToList();
        }

        public ItemOfStuff GetByStuffId(string stuffId)
        {
            // TODO: Prod app would need some error handling like for bad/deleted Id's etc.
            return _sqlConnection.Get<ItemOfStuff>(stuffId);
        }

        // TODO: Reminder about all current storage/repositories in here
        // TODO: Use SQLite as xplat file format to store "Event Stream blobs" maybe?
        // TODO: Do we want things that NEVER get deleted like events stored on a phone?
        // TODO: How would/could you be a good mobile-client citizen using ES on a client without infinite storage?
        // TODO: If I use SQLite "relational" approach, remember when I "Delete" an Action/Project, etc.
        // TODO: I have to manually deal with the ref integrity/cascading deletes, etc.

        public void TrashStuff(ItemOfStuff itemOfStuff)
        {
            _sqlConnection.Delete(itemOfStuff);
        }
    }
}
