using System.Collections.Generic;
using System.Linq;
using Gtd.Shell.Actions;

namespace Gtd.Shell
{
    public interface IConsoleAction
    {
        string Usage { get; }
        void Execute(ConsoleEnvironment env, string[] args);
    }

    public static class ConsoleActions
    {
        public static IDictionary<string, IConsoleAction> Actions = new Dictionary<string, IConsoleAction>();

        static ConsoleActions()
        {
            Register(new ExitAction());
            Register(new HelpAction());


            Register(new AddAction());
            //Register(new OpenFactoryAction());
            //Register(new RegisterBlueprintAction());
            //Register(new HireEmployeeAction());
            //Register(new RecieveShipmentAction());
            //Register(new UnpackShipmentsAction());
            
            
            //Register(new StoryAction());
        }
        static void Register(IConsoleAction cmd)
        {
            Actions.Add(cmd.Usage.Split(new[] { ' ' }, 2).First(), cmd);
        }
    }
}