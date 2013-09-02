﻿using System.Collections.Generic;
using Gtd.Client.Core.Models;

namespace Gtd.Client.Core.Services.Actions
{
    public interface IActionService
    {
        IList<Action> AllActions();
        void DefineAction(Action action);
        Action GetByActionId(string actionId);
        Action GetByProjectId(string projectId);
    }
}

