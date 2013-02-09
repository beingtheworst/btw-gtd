#region (c) 2010-2011 Lokad - CQRS for Windows Azure - New BSD License 

// Copyright (c) Lokad 2010-2011, http://www.lokad.com
// This code is released as Open Source under the terms of the New BSD Licence

#endregion

using System;

namespace Gtd.Shell.Commands
{
    class DefineProjectCommand : IConsoleCommand
    {
        public string Usage
        {
            get { return @"project [project outcome]
    Define new project with a given outcome"; }
        }

        public string[] Key { get { return new[] {"project", "dp"}; } }

        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must provide project outcome");
                return;
            }

            var outcome = string.Join(" ", args);
            env.TrustedSystem.When(new DefineProject(env.Session.SystemId, Guid.Empty, outcome));
            env.Log.Info("Project defined!");
        }
    }
}