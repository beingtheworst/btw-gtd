﻿using System;
using System.Collections.Generic;

namespace Gtd.Pcl.CoreDomain.AppServices.TrustedSystem
{
    /// <summary> This is the current in-memory "state" half of our A+ES implementation.
    /// 
    /// We split the Aggregates and Event Sourcing (A+ES) implementation
    /// into two distinct classes:
    /// - one for state (AggregateNameState),
    /// - and one for behavior (AggregateName),
    /// with the state object (this class) being held by the behavioral object (the Aggregate).
    /// 
    /// The two objects collaborate exclusively through the Aggregate's Apply() method.
    /// This ensures that an Aggregate's state is changed only by means of Events.
    /// This AggregateState class typically exists for very short periods at a time
    /// (likely milliseconds) when the Aggregate is loaded from Event Stream history and
    /// the Events are replayed through this state class to derive current Aggregate state.
    /// This current in-memory state ensures that Aggregate behaviors (methods)
    /// with multiple steps, have up-to-date state to operate on for each subsequent step.
    /// 
    /// When changes are made to an Aggregate, and the results are persisted back to 
    /// the Event Stream by the ApplicationService, the system drops all references 
    /// to this state class instance and it is garbage collected.
    /// </summary>
    public sealed class TrustedSystemState : ITrustedSystemState
    {
        public TrustedSystemState()
        {
        }

        public TrustedSystemId Id { get; private set; }

        public static TrustedSystemState BuildStateFromEventHistory(IEnumerable<Event> events)
        {
            var aggState = new TrustedSystemState();

            foreach (var eventThatHappened in events)
            {
                aggState.MakeStateRealize((ITrustedSystemEvent) eventThatHappened);
            }
            return aggState;
        }

        public void MakeStateRealize(ITrustedSystemEvent thisEventTypeHappened)
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


        // When methods reacting to Events to change in-memory state of the Aggregate

        public void When(TrustedSystemCreated e)
        {
            Id = e.Id;
        }

        public void When(StuffPutInInbox e)
        {
            var info = new StuffInfo(e.StuffId, e.StuffDescription);

            // Store the details of this stuff in this dictionary
            StuffInInbox.Add(e.StuffId, info);
            
            // Now Add StuffId to our Inbox HashSet for easy lookup of our "Stuff"
            Inbox.Add(e.StuffId);
        }

        public void When(StuffDescriptionChanged e)
        {
            StuffInInbox[e.StuffId].ChangeDescription(e.NewDescriptionOfStuff);
        }

        public void When(StuffTrashed e)
        {
            // Stop tracking this Stuff from our Inbox HashSet
            Inbox.Remove(e.StuffId);
        }

        public void When(StuffArchived e)
        {
            // Stop tracking this Stuff from our Inbox HashSet
            Inbox.Remove(e.StuffId);
        }

        public void When(ProjectDefined e)
        {
            Projects.Add(e.ProjectId, new ProjectInfo(e.ProjectId, e.ProjectOutcome, e.Type));
        }

        public void When(ProjectOutcomeChanged e)
        {
            Projects[e.ProjectId].ChangeOutcome(e.ProjectOutcome);
        }

        public void When(ProjectTypeChanged e)
        {
            Projects[e.ProjectId].ChangeType(e.Type);
        }

        public void When(ActionDefined e)
        {
            
            Actions.Add(e.ActionId, new ActionInfo(e.ActionId, e.Outcome, e.ProjectId));
        }

        // TODO: Nothing generates this Event yet
        public void When(ActionAssignedToProject e)
        {
            var action = Actions[e.ActionId];
            var project = Projects[e.NewProject];

            action.LinkToProject(project.Id);
            project.AddAction(action.Id);
        }

        // TODO: Nothing generates this Event yet
        public void When(ActionRemovedFromProject e)
        {
            Actions[e.ActionId].RemoveFromProject(e.OldProject);
            Projects[e.OldProject].RemoveAction(e.ActionId);
        }
        
        public void When(ActionMovedToProject e)
        {
            var action = Actions[e.ActionId];
            var oldProject = Projects[e.OldProject];
            var newProject = Projects[e.NewProject];

            action.MoveToProject(e.OldProject, e.NewProject);
            
            newProject.AddAction(action.Id);
            oldProject.RemoveAction(action.Id);
        }

        public void When(ActionArchived e)
        {
            Actions[e.ActionId].MakeArchived();
        }

        public void When(ActionOutcomeChanged e)
        {
            Actions[e.ActionId].ChangeOutcome(e.ActionOutcome);
        }

        public void When(ActionDeferredUntil e)
        {
            Actions[e.ActionId].DeferredUntil(e.DeferUntil);
        }

        public void When(ActionDeferDateShifted e)
        {
            Actions[e.ActionId].DeferDateShifted(e.OldDeferDate, e.NewDeferDate);
        }

        public void When(ActionIsNoLongerDeferred e)
        {
            Actions[e.ActionId].NoLongerDeferred(e.OldDeferDate);
        }

        public void When(DueDateAssignedToAction e)
        {
            Actions[e.ActionId].DueDateAssigned(e.NewDueDate);
        }

        public void When(ActionDueDateMoved e)
        {
            Actions[e.ActionId].DueDateMoved(e.OldDueDate, e.NewDueDate);
        }

        public void When(DueDateRemovedFromAction e)
        {
            Actions[e.ActionId].DueDateRemoved(e.OldDueDate);
        }

        public void When(ActionCompleted e)
        {
            Actions[e.ActionId].MarkAsCompleted();
        }

        // The When methods in this transient state class are reacting to 
        // Events, and this state class tends to contain a lot of 
        // Entities (things that have Ids we care about).
        // We see some of these Ids acting as keys to the Dictionaries below.

        public readonly HashSet<StuffId> Inbox = new HashSet<StuffId>();
        public readonly IDictionary<StuffId, StuffInfo> StuffInInbox = new Dictionary<StuffId, StuffInfo>();
        public readonly IDictionary<ProjectId, ProjectInfo> Projects = new Dictionary<ProjectId, ProjectInfo>();
        public readonly IDictionary<ActionId, ActionInfo> Actions = new Dictionary<ActionId, ActionInfo>();

    }   // end TrustedSystemState


    /// <summary> These "Info" objects maintain only invariants within themselves.
    /// Invariants between entities are maintained by the state. This helps to achieve
    /// more encapsulated code. This code is pleasant to work with and we know
    /// that there will be no side effects. Rules/biz logic can be enforced in this class
    /// and they will not be broken by outside or new parties/developers to the code.
    /// (ex: if Action is Archived then its current Completion state must never change)
    /// </summary>
    public sealed class StuffInfo
    {
        public StuffId Id { get; private set; }
        public string Description { get; private set; }

        public StuffInfo(StuffId id, string stuffDescription)
        {
            Enforce.NotEmpty(stuffDescription, "stuffDescription");

            Id = id;
            Description = stuffDescription;
        }

        public void ChangeDescription(string newDescriptionOfStuff)
        {
            Enforce.NotEmpty(newDescriptionOfStuff, "newDescriptionOfStuff");

            Description = newDescriptionOfStuff;
        }
    }

    public sealed class ProjectInfo
    {
        public ProjectId Id { get; private set; }
        public string Outcome { get; private set; }
        public ProjectType Type { get; private set; }


        readonly List<ActionId> _actions = new List<ActionId>();


        public ProjectInfo(ProjectId id, string name, ProjectType type)
        {
            Enforce.NotEmpty(name, "name");

            Id = id;
            Outcome = name;
            Type = type;
        }

        public void ChangeType(ProjectType type)
        {
            Type = type;
        }

        public void ChangeOutcome(string newName)
        {
            Outcome = newName;
        }

        public void AddAction(ActionId action)
        {
            _actions.Add(action);
        }

        public void RemoveAction(ActionId action)
        {
            _actions.Remove(action);
        }
    }

    #region ActionInfo is an example of an Entity class that represents a GTD Action...
    // that only exists within this transient state class for brief moments at a time.
    // It is one of the multiple possible representations of a GTD Action in the system.
    // (another may be a View of a GTD Action that only exists in the console)
    // ActionInfo is implemented using the CQS pattern. ActionInfo and the other
    // "Info" classes below have no publically settable properties.
    #endregion
    public sealed class ActionInfo
    {
        public ActionId Id { get; private set; }
        public string Outcome { get; private set; }
        public ProjectId Project { get; private set; }

        public DateTime DeferUntil { get; private set; }

        public DateTime DueDate { get; private set; }

        public bool HasProject { get { return !Project.IsEmpty; } }

        public bool Completed { get; private set; }
        public bool Archived { get; private set; }

        public ActionInfo(ActionId id, string name, ProjectId owner)
        {
            Id = id;
            Outcome = name;
            Project = owner;
        }

        public void MakeArchived()
        {
            Archived = true;
        }

        public void LinkToProject(ProjectId id)
        {
            if (HasProject)
                throw new InvalidOperationException("Action already assigned to a project");
            Project = id;

        }

        public void MarkAsCompleted()
        {
            if (Completed)
                throw new InvalidOperationException("Action is already completed");
            Completed = true;
        }

        public void MoveToProject(ProjectId oldProject, ProjectId newProject)
        {
            if (oldProject != Project)
                throw new InvalidOperationException("OldProject != CurrentProject");
            if (newProject == ProjectId.Empty)
                throw new InvalidOperationException("NewProject == Empty");
            Project = newProject;
        }

        public void RemoveFromProject(ProjectId project)
        {
            if (project != Project)
                throw new InvalidOperationException("OldProject != CurrentProject");
            Project = ProjectId.Empty;
        }

        public void ChangeOutcome(string newName)
        {
            Outcome = newName;
        }
      
        public void EnsureCleanRemoval()
        {
            if (HasProject)
                throw new InvalidOperationException("Can't remove action that is still assigned to project");
        }

        public void DeferDateShifted(DateTime oldTime, DateTime newTime)
        {
            if (DeferUntil != oldTime)
                throw new ArgumentException("Expected old time matching to actual date");
            DeferUntil = newTime;
        }

        public void DeferredUntil(DateTime deferDate)
        {
            if (DeferUntil != DateTime.MinValue)
                throw new ArgumentException("Expected null date");
            DeferUntil = deferDate;
        }

        public void NoLongerDeferred(DateTime startDate)
        {
            if (DeferUntil != startDate)
                throw new ArgumentException("Expected old time matching to actual date");
            DeferUntil = DateTime.MinValue;
        }

        public void DueDateRemoved(DateTime dueDate)
        {
            if (DueDate != dueDate)
                throw new ArgumentException("Expected old time matching to actual date");
            DueDate = DateTime.MinValue;
        }

        public void DueDateMoved(DateTime oldTime, DateTime newTime)
        {
            if (DueDate != oldTime)
                throw new ArgumentException("Expected old time matching to actual date");
            DueDate = newTime;
        }

        public void DueDateAssigned(DateTime dueDate)
        {
            if (DueDate != DateTime.MinValue)
                throw new ArgumentException("Expected null date");
            DueDate = dueDate;
        }
    }
}
