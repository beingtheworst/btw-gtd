using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.Tenant
{
    public sealed class TenantState
    {
        public static TenantState BuildStateFromHistory(IEnumerable<Event> events)
        {
            var state = new TenantState();
            foreach (var e in events)
            {
                state.MakeStateRealizeThat(e);
            }
            return state;
        }
        

        public void MakeStateRealizeThat(Event thisEventTypeHappened)
        {
            #region What Is This Code/Syntax Doing?
            // After the Aggregate records the Event, we announce this Event to all those
            // that care about it to help them Realize that things have changed.
            // We are telling the compiler to call one of the "When" methods defined above to achieve this realization.
            // The "dynamic" syntax below is just a shortcut we are using so we don't
            // have to create a large if/else block for a bunch of type-specific static Event types.
            // This shortcut using the "dynamic" keyword syntax means:
            // "Call 'this' Aggregates's instance of the 'When' method
            // that has a method signature of:
            // When(WhateverTheCurrentEventTypeIsOf-thisEventTypeHappened)".
            #endregion

            ((dynamic)this).When((dynamic)thisEventTypeHappened);
        }
    }
}