using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.WebHost.Endpoints;

namespace Gtd.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting GTD server in console mode. Press enter to quit");
            Console.WriteLine("This is an empty stub now");
            
            Console.WriteLine("Make sure, that you are running as ADMIN!");
            using (var host = new ServiceStackHost())
            {
                host.Init();
                host.Start("http://localhost:20002/");
                Console.ReadLine();
            }
        }
    }

    public class ServiceStackHost : AppHostHttpListenerBase
    {

        public ServiceStackHost()
            : base("BTW GTD (raw)", typeof(ServiceStackHost).Assembly)
        {
            
        }

        public override void Configure(Funq.Container container)
        {
            //LoadPlugin(new TaskSupport());
            //Routes
            //    .Add<ClientDto.WriteEvent>(ClientDto.WriteEvent.Url, "POST")
            //    .Add<ClientDto.WriteBatch>(ClientDto.WriteBatch.Url, "POST")
            //    .Add<ClientDto.ResetStore>(ClientDto.ResetStore.Url, "POST")
            //    .Add<ClientDto.ShutdownServer>(ClientDto.ShutdownServer.Url, "GET");

            //container.Register(_publisher);
        }
    }

}
