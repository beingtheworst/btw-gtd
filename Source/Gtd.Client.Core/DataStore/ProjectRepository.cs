using System.Collections.Generic;
using System.Linq;
using Cirrious.MvvmCross.Plugins.Sqlite;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public class ProjectRepository : IProjectRepository
    {
        // TODO: wrap this in a using instead/later
        private readonly ISQLiteConnection _sqlConnection;

        public ProjectRepository(ISQLiteConnectionFactory sqlConnectionFactory)
        {
            // use factory to get the connection to SQLite database
            // pass create the name of the SQLite database
            _sqlConnection = sqlConnectionFactory.Create("btwgtd.sqlite");

            // Create or ensure the dbase Table is created for our Project
            _sqlConnection.CreateTable<Project>();
        }

        public void DefineProject(Project project)
        {
            _sqlConnection.Insert(project);
        }

        public IList<Project> AllProjects()
        {
            return _sqlConnection
                .Table<Project>()
                .OrderBy(proj => proj.Outcome)
                .ToList();
        }

        public Project GetByProjectId(string projectId)
        {
            // TODO: Prod app would need some error handling like for bad/deleted Id's etc.
            return _sqlConnection.Get<Project>(projectId);
        }
    }
}

