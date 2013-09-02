using System.Collections.Generic;

namespace Gtd.Pcl.CoreDomain.AppServices.ClientProfile
{
    public sealed class ClientProfileAggregate
    {
        public readonly IList<Event> EventsCausingChanges = new List<Event>();

        readonly ClientProfileState _aggState;

        public ClientProfileAggregate(ClientProfileState aggStateBeforeChanges)
        {
            _aggState = aggStateBeforeChanges;
        }


        /// <summary> Make Aggregate realize the event happened 
        /// by applying it to the state and adding to 
        /// the list of uncommitted events</summary>
        /// <param name="eventThatHappened"></param>
        void Apply(IClientProfileEvent eventThatHappened)
        {
            // update Agg's in-memory state so if a behavior (method) has 
            // multiple steps, each subsequent step has up-to-date state to operate on
            _aggState.MakeStateRealize(eventThatHappened);

            // update Agg's public collection of change causing Events so the 
            // AppService can use it to persist AggState as appended Events to this Agg's Event Stream
            EventsCausingChanges.Add((Event)eventThatHappened);
        }

        public void InitIfNeeded()
        {
            if (_aggState.CurrentSystem == null)
            {
                // TODO: generate proper ID somehow - still hard-coded to 1 for now!
                Apply(new ClientProfileSwitchedToTrustedSystem(new TrustedSystemId(1)));
            }

        }

        public TrustedSystemId GetCurrentSystemId()
        {
            return _aggState.CurrentSystem;
        }
    }
}
