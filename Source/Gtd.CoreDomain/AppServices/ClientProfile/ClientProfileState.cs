using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.ClientProfile
{
    public sealed class ClientProfileState : IClientProfileState
    {


        public TrustedSystemId CurrentSystem { get; private set; }



        public void When(ClientProfileSwitchedToTrustedSystem e)
        {
            CurrentSystem = e.Id;
        }

        public static ClientProfileState BuildStateFromEventHistory(IEnumerable<Event> events)
        {
            var aggState = new ClientProfileState();

            foreach (var eventThatHappened in events)
            {
                aggState.MakeStateRealize((IClientProfileEvent)eventThatHappened);
            }
            return aggState;
        }

        public void MakeStateRealize(IClientProfileEvent thisEventTypeHappened)
        {
            #region What Is This Code/Syntax Doing?
            // Announce that a specific Type of Event Message occured
            // so that this AggregateState instance can Realize it and change its state because of it.
            // We are telling the compiler to call one of the "When" methods in this class to achieve this realization.
            // The "dynamic" syntax below is just a shortcut we are using so we don't
            // have to create a large if/else block for a bunch of type-specific static Event types.
            // This shortcut using the "dynamic" keyword syntax means:
            // "Call the 'When' method of 'this' AggregateState instance
            // that has a method signature of:
            // When(WhateverTheEventTypeIsOf-thisEventTypeHappened)".
            #endregion

            ((dynamic)this).When((dynamic)thisEventTypeHappened);
        }
    }
}