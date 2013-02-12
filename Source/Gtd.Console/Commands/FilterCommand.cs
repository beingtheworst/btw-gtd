namespace Gtd.Shell.Commands
{
    public sealed class FilterCommand : IConsoleCommand
    {
        public string[] Key { get { return new[] { "filter","f"}; } }
        public string Usage
        {
            get { return @"f [<optional filter id>]
    Display currently selected action filter or switch filter to one of the available."; }
        }
        public void Execute(ConsoleEnvironment env, string[] args)
        {
            if (args.Length == 0)
            {
                env.Log.Info("Current filter: {0}", env.Session.CurrentFilter.Title);

                for (int i = 0; i < env.Filters.Count; i++)
                {
                    var filter = env.Filters[i];
                    env.Log.Trace("  {0}. {1}", i+1, filter.Title);
                }
                
                return;
            }
            int filterId = 1;

            if (args.Length == 1 && int.TryParse(args[0], out filterId))
            {
                var filter = env.Filters[filterId-1];
                env.Session.UpdateFilter(filter);
                env.Log.Trace("Changed filter to {0}", filter.Title);
            }
        }
    }
}