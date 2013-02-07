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
            var projectId = new ProjectId(NewGuidIfEmpty(requestId));

            Apply(new ProjectDefined(_aggState.Id, projectId, name, time));
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
            var actionId = new ActionId(NewGuidIfEmpty(requestId));

            Apply(new ActionDefined(_aggState.Id, actionId, actionName, time));
        }

        public void ArchiveThought(Guid thoughtId, ITimeProvider provider)
        {
            if (!_aggState.Inbox.Contains(thoughtId))
                throw DomainError.Named("no thought", "Thought {0} not found", thoughtId);

            Apply(new ThoughtArchived(_aggState.Id, thoughtId, provider.GetUtcNow()));
        }


        // Helper Methods

        static Guid NewGuidIfEmpty(Guid requestId)
        {
            return requestId == Guid.Empty ? Guid.NewGuid() : requestId;
        }

        void Apply(ITenantEvent newEventThatHappened)
        {
            // TODO: [kstreet] In the Factory sample these two lines of code were called
            // the other way around (MakeStateRealize was called AFTER the newEventThatHappened was added to List)
            // Does it matter?  Is one approach a little safer/more accurate than the other?

            _aggState.MakeStateRealize(newEventThatHappened);
            EventsThatCausedChange.Add((Event)newEventThatHappened);
        }

    }
}