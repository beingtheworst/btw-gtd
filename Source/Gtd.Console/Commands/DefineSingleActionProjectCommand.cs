using System;
using Gtd.Shell.Projections;

namespace Gtd.Shell.Commands
{
    class DefineSingleActionProjectCommand : IConsoleCommand
    {

        public string[] Key { get { return new string[] { "dsap", "sap" }; } }
        public string Usage { get { return @"dsap <thought-id>
    Define new single action project and its action from an existing thought"; } }

        
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Error("You must provide an existing thought id");
                return;
            }

            var record = env.Session.MatchItem(args[0]);

            var thought = record as ThoughtView;

            if (thought != null)
            {
                var requestId = new RequestId(Guid.NewGuid());
                env.TrustedSystem.When(new DefineSingleActionProject(env.Session.SystemId, requestId, thought.Id));
                env.Log.Info("Single Action Project defined from a thought!");
                return;
            }
        }
    }
}
