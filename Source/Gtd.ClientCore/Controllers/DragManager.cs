using System;
using System.Collections.Immutable;
using Gtd.Client.Models;
using System.Linq;

namespace Gtd.Client
{
    public abstract class DragManager
    {
        public readonly string Request;

        protected DragManager(string request)
        {
            Request = request;
        }

        public abstract bool CanDropToProject(string requestId, ProjectId id);

        public virtual void DropToProject(string requestId, ProjectId id)
        {
            
        }
    }

    public sealed class ActionDragManager : DragManager
    {
        readonly IImmutableList<ImmutableAction> _actions;
        readonly IMessageQueue _queue;

        public ActionDragManager(string request, IImmutableList<ImmutableAction> action, IMessageQueue queue) : base(request)
        {
            _actions = action;
            _queue = queue;
        }

        public override bool CanDropToProject(string requestId, ProjectId id)
        {
            if (Request != requestId)
                return false;


            //if (_action.ProjectId.Id == id.Id)
            //    return false;
            //return true;
            return true;
        }
        public override void DropToProject(string requestId, ProjectId id)
        {
            _queue.Enqueue(new UI.MoveActionsToProject(_actions.Select(a => a.ActionId).ToImmutableList(), id));
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

        public override bool CanDropToProject(string requestId, ProjectId id)
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