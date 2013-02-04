using System;
using System.Collections.Generic;
using System.Linq;

namespace Gtd.CoreDomain.Tests.AppService
{
    /// <summary>
    /// Base class that acts as execution environment (container) for our application 
    /// service. It will be responsible for wiring in test version of services to
    /// factory and executing commands
    /// </summary>
    public abstract class tenant_application_service_spec : application_service_spec
    {
        //public TestBlueprintLibrary Library;

        protected override void SetupServices()
        {
          //  Library = new TestBlueprintLibrary();
        }

        protected override Event[] ExecuteCommand(Event[] given, Command cmd)
        {
            var store = new SingleCommitMemoryStore();
            foreach (var e in given.OfType<ITenantEvent>())
            {
                store.Preload(e.Id.ToString(), (Event) e);
            }
            new TenantAppService(store, null).Execute(cmd);
            return store.Appended ?? new Event[0];
        }


        protected void When(ITenantCommand when)
        {
            WhenMessage((Command) when);
        }
        protected void Given(params ITenantEvent[] given)
        {
            GivenMessages(given.Cast<Event>());
        }
        protected void GivenSetup(params SpecSetupEvent[] setup)
        {
            GivenMessages(setup);
        }
        protected void Expect(params ITenantEvent[] given)
        {
            ExpectMessages(given.Cast<Event>());
        }// additional helper builders
    }

    public static class ExtendArray
    {
        public static T[] Cast<T>(this Array source)
        {

            var result = new T[source.Length];
            for (int i = 0; i < source.Length; i++)
            {
                result[i] = (T) source.GetValue(i);
            }
            return result;
        }
    }
    sealed class SingleCommitMemoryStore : IEventStore
    {
        public readonly IList<Tuple<string, Event>> Store = new List<Tuple<string, Event>>();
        public Event[] Appended = null;
        public void Preload(string id, Event e)
        {
            Store.Add(Tuple.Create(id, e));
        }

        EventStream IEventStore.LoadEventStream(string id)
        {
            var events = Store.Where(i => id.Equals(i.Item1)).Select(i => i.Item2).ToList();
            return new EventStream
            {
                Events = events,
                StreamVersion = events.Count
            };
        }

        void IEventStore.AppendEventsToStream(string id, long expectedVersion, ICollection<Event> events)
        {
            if (Appended != null)
                throw new InvalidOperationException("Only one commit it allowed");
            Appended = events.ToArray();
        }
    }

}