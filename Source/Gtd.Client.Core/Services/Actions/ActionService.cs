using System.Collections.Generic;
using Gtd.Client.Core.DataStore;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Actions
{
    // We use this service so we can have singletons holding all
    // the stuff in the inbox in memory so that the ViewModels are not
    // constantly hitting the database layer to get it

    // probably going to be a Lazy Singleton per config convention in App.cs "Service"
    public class ActionService : IActionService
    {
        readonly IActionRepository _actionRepository;

        public ActionService(IActionRepository actionRepository)
        {
            // this wont get initialized from the Mvx IoC
            // so we do it here
            _actionRepository = actionRepository;

        }

        // TODO: May want to do validation in here instead of ViewModel
        public void DefineAction(Action action)
        {
            _actionRepository.DefineAction(action);
        }

        public IList<Action> AllActions()
        {
            // this will likely get cached down the road but use All for now
            return _actionRepository.AllActions();
        }

        public Action GetByActionId(string actionId)
        {
            return _actionRepository.GetByActionId(actionId);
        }

        public Action GetByProjectId(string projectId)
        {
            return _actionRepository.GetByProjectId(projectId);
        }
    }
}
