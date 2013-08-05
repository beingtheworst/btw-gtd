using System;

namespace Gtd.Client
{
    public abstract class DragManager
    {
        public readonly string Request;

        protected DragManager(string request)
        {
            Request = request;
        }

        public abstract bool CanDropToProject(string requestId);

        public virtual void DropToProject(string requestId, ProjectId id)
        {
            
        }
    }

    public sealed class StuffDragManager : DragManager
    {
        public readonly StuffId Stuff;
        // TODO: replace with envelope
        readonly IMessageQueue _queue;

        public StuffDragManager(string request, StuffId stuff, IMessageQueue queue) : base(request)
        {
            Stuff = stuff;
            _queue = queue;
        }

        public override bool CanDropToProject(string requestId)
        {
            if (Request != requestId) return false;
            return true;
        }

        public override void DropToProject(string requestId, ProjectId id)
        {
            _queue.Enqueue(new UI.MoveStuffToProjectClicked(new[]{Stuff, },id ));
        }
    }

    
}