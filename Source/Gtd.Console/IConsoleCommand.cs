using System.Collections.Generic;
using System.Linq;
using Gtd.Shell.Commands;

namespace Gtd.Shell
{
    public interface IConsoleCommand
    {
        string Usage { get; }
        void Execute(ConsoleEnvironment env, string[] args);
    }

    public static class ConsoleCommands
    {
        public static IDictionary<string, IConsoleCommand> Actions = new Dictionary<string, IConsoleCommand>();

        static ConsoleCommands()
        {
            Register(new ExitCommand());
            Register(new HelpCommand());


            Register(new AddActionCommand());
            //Register(new OpenFactoryAction());
            //Register(new RegisterBlueprintAction());
            //Register(new HireEmployeeAction());
            //Register(new RecieveShipmentAction());
            //Register(new UnpackShipmentsAction());
            
            
            //Register(new StoryAction());
        }
        static void Register(IConsoleCommand cmd)
        {
            Actions.Add(cmd.Usage.Split(new[] { ' ' }, 2).First(), cmd);
        }
    }
}