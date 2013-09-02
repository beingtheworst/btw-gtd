using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.Plugins.Sqlite;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public class ActionRepository : IActionRepository
    {
        // TODO: wrap this in a using instead/later
        private readonly ISQLiteConnection _sqlConnection;

        public ActionRepository(ISQLiteConnectionFactory sqlConnectionFactory)
        {
            // use factory to get the connection to SQLite database
            // pass create the name of the SQLite database
            _sqlConnection = sqlConnectionFactory.Create("btwgtd.sqlite");

            // Create or ensure the dbase Table is created for our Action
            _sqlConnection.CreateTable<Action>();
        }

        public void DefineAction(Action action)
        {
            _sqlConnection.Insert(action);
        }

        public IList<Action> AllActions()
        {
            return _sqlConnection
                .Table<Action>()
                .OrderBy(action => action.Outcome)
                .ToList();
        }

        public Action GetByActionId(string actionId)
        {
            // TODO: Prod app would need some error handling like for bad/deleted Id's etc.
            return _sqlConnection.Get<Action>(actionId);
        }

        public Action GetByProjectId(string projectId)
        {
            // TODO: Prod app would need some error handling like for bad/deleted Id's etc.
            return _sqlConnection.Get<Action>(projectId);
        }
    }
}
