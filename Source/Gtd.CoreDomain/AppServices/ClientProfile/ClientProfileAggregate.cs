using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.ClientProfile
{
    public sealed class ClientProfileAggregate
    {
        public readonly IList<Event> Changes = new List<Event>();

        readonly ClientProfileState _state;

        public ClientProfileAggregate(ClientProfileState state)
        {
            _state = state;
        }


        /// <summary> Make Aggregate realize the event happened 
        /// by applying it to the state and adding to 
        /// the list of uncommitted events</summary>
        /// <param name="newEventThatHappened"></param>
        void Apply(IClientProfileEvent newEventThatHappened)
        {
            // update Agg's in-memory state so if a behavior (method) has 
            // multiple steps, each subsequent step has up-to-date state to operate on
            _state.MakeStateRealize(newEventThatHappened);

            // update Agg's public collection of change causing Events so the 
            // AppService can use it to persist AggState as appended Events to this Agg's Event Stream
            Changes.Add((Event)newEventThatHappened);
        }

        public void InitIfNeeded()
        {
            if (_state.CurrentSystem == null)
            {
                // TODO: generate proper ID somehow
                Apply(new ClientProfileSwitchedToTrustedSystem(new TrustedSystemId(1)));
            }

        }

        public TrustedSystemId GetCurrentSustemId()
        {
            return _state.CurrentSystem;
        }
    }
}