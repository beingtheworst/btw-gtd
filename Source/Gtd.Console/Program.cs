using System;
using System.Text;
using Gtd.CoreDomain;

namespace Gtd.Console
{
    class Program
    {
        static void Main(string[] args)
        {
 
        }
    }

    public sealed class ConsoleEnvironment
    {

        public InMemoryStore Store { get; private set; }
        public ITenantApplicationService Tenant { get; private set; }
        public static ConsoleEnvironment Build()
        {
            var handler = new SynchronousEventHandler();
            var store = new InMemoryStore(handler);
            var tenant = new TenantAppService(store);

            
            return new ConsoleEnvironment()
                {
                    Store = store,
                    Tenant = tenant,
                };
            
        }
    }
}
