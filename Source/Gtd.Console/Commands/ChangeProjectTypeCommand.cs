namespace Gtd.Shell.Commands
{
    public sealed class ChangeProjectTypeCommand : IConsoleCommand
    {
        public string[] Key { get { return new string[] { "type", "cp"};} }
        public string Usage { get { return @"type <projectId> seq | si | par
    Change project type"; } }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length != 2)
            {
                throw new KnownConsoleInputError("Expected 2 arguments");
            }

            var project = env.Session.MatchProject(args[0]);
            ProjectType  changeTo;

            var lowerInvariant = args[1].ToLowerInvariant();
            if (lowerInvariant.StartsWith("p"))
            {
                changeTo = ProjectType.Parallel;
            }
            else if (lowerInvariant.StartsWith("se"))
            {
                changeTo = ProjectType.Sequential;
            }
            else if (lowerInvariant.StartsWith("si"))
            {
                changeTo = ProjectType.SingleActions;
            }
            else
            {
                var message = string.Format("Unknown project type {0}", lowerInvariant);
                throw new KnownConsoleInputError(message);
            }


            env.TrustedSystem.When(new Gtd.ChangeProjectType(env.Session.SystemId, project.ProjectId, changeTo));
            env.Log.Trace("Projec type changed");
        }
    }
}