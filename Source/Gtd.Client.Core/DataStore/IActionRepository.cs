using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.DataStore
{
    public interface IActionRepository
    {
        IList<Action> AllActions();
        void DefineAction(Action action);
        Action GetByActionId(ActionId actionId);
        Action GetByProjectId(ProjectId projectId);
    }
}
