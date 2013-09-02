using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public interface IActionRepository
    {
        // SQLite doesn't like my custom Id types. Goign to string for now.
        IList<Action> AllActions();
        void DefineAction(Action action);
        //Action GetByActionId(ActionId actionId);
        //Action GetByProjectId(ProjectId projectId);
        Action GetByActionId(string actionId);
        Action GetByProjectId(string projectId);
    }
}
