using System;
using System.Collections.Generic;

namespace Gtd.CoreDomain.AppServices.Tenant
{
    public sealed class TenantAggregate
    {
        readonly TenantState _aggState;

        public List<Event> EventsThatCausedChange = new List<Event>();

        public TenantAggregate(TenantState aggregateStateBeforeChanges)
        {
            _aggState = aggregateStateBeforeChanges;
        }

        // Aggregate Behaviors (Methods)

        public void Create(TenantId id)
        {
            Apply(new TenantCreated(id));
        }

        public void DefineProject(Guid requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var id = new ProjectId(NewGuidIfEmpty(requestId));

            Apply(new ProjectDefined(_aggState.Id, id, name, time));
        }

        public void CaptureThought(Guid requestId, string name, ITimeProvider provider)
        {
            // filter request IDs
            //var time = provider.GetUtcNow();
            //var id = new ActionId(NewGuidIfEmpty(requestId));

            Apply(new ThoughtCaptured(_aggState.Id, NewGuidIfEmpty(requestId), name, provider.GetUtcNow()));
        }

        public void DefineAction(Guid requestId, string actionName, ITimeProvider provider)
        {
            // filter request IDs
            var time = provider.GetUtcNow();
            var id = new ActionId(NewGuidIfEmpty(requestId));

            Apply(new ActionDefined(_aggState.Id, id, actionName, time));
        }

        public void ArchiveThought(Guid thoughtId, ITimeProvider provider)
        {
            if (!_aggState.Inbox.Contains(thoughtId))
                throw DomainError.Named("no thought", "Thought {0} not found", thoughtId);

            Apply(new ThoughtArchived(_aggState.Id, thoughtId, provider.GetUtcNow()));
        }


        // Helper Methods

        static Guid NewGuidIfEmpty(Guid request)
        {
            return request == Guid.Empty ? Guid.NewGuid() : request;
        }

        void Apply(ITenantEvent newEventThatHappened)
        {
            _aggState.MakeStateRealizeThat(newEventThatHappened);
            EventsThatCausedChange.Add((Event)newEventThatHappened);
        }

    }
}