﻿using System;
using System.Runtime.Serialization;

// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable UnusedMember.Local
namespace Gtd
{
    #region Generated by Lokad Code DSL
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class TrustedSystemCreated : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        
        TrustedSystemCreated () {}
        public TrustedSystemCreated (TrustedSystemId id)
        {
            Id = id;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class PutStuffInInbox : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public RequestId RequestId { get; private set; }
        [DataMember(Order = 3)] public string StuffDescription { get; private set; }
        
        PutStuffInInbox () {}
        public PutStuffInInbox (TrustedSystemId id, RequestId requestId, string stuffDescription)
        {
            Id = id;
            RequestId = requestId;
            StuffDescription = stuffDescription;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class StuffPutInInbox : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public StuffId StuffId { get; private set; }
        [DataMember(Order = 3)] public string StuffDescription { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        StuffPutInInbox () {}
        public StuffPutInInbox (TrustedSystemId id, StuffId stuffId, string stuffDescription, DateTime timeUtc)
        {
            Id = id;
            StuffId = stuffId;
            StuffDescription = stuffDescription;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class TrashStuff : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public StuffId StuffId { get; private set; }
        
        TrashStuff () {}
        public TrashStuff (TrustedSystemId id, StuffId stuffId)
        {
            Id = id;
            StuffId = stuffId;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class StuffTrashed : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public StuffId StuffId { get; private set; }
        [DataMember(Order = 3)] public DateTime TimeUtc { get; private set; }
        
        StuffTrashed () {}
        public StuffTrashed (TrustedSystemId id, StuffId stuffId, DateTime timeUtc)
        {
            Id = id;
            StuffId = stuffId;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class StuffArchived : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public StuffId StuffId { get; private set; }
        [DataMember(Order = 3)] public DateTime TimeUtc { get; private set; }
        
        StuffArchived () {}
        public StuffArchived (TrustedSystemId id, StuffId stuffId, DateTime timeUtc)
        {
            Id = id;
            StuffId = stuffId;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class DefineAction : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public RequestId RequestId { get; private set; }
        [DataMember(Order = 3)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 4)] public string Outcome { get; private set; }
        
        DefineAction () {}
        public DefineAction (TrustedSystemId id, RequestId requestId, ProjectId projectId, string outcome)
        {
            Id = id;
            RequestId = requestId;
            ProjectId = projectId;
            Outcome = outcome;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionDefined : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 4)] public string Outcome { get; private set; }
        [DataMember(Order = 5)] public DateTime TimeUtc { get; private set; }
        
        ActionDefined () {}
        public ActionDefined (TrustedSystemId id, ActionId actionId, ProjectId projectId, string outcome, DateTime timeUtc)
        {
            Id = id;
            ActionId = actionId;
            ProjectId = projectId;
            Outcome = outcome;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class DefineProject : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public RequestId RequestId { get; private set; }
        [DataMember(Order = 3)] public string ProjectOutcome { get; private set; }
        
        DefineProject () {}
        public DefineProject (TrustedSystemId id, RequestId requestId, string projectOutcome)
        {
            Id = id;
            RequestId = requestId;
            ProjectOutcome = projectOutcome;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ProjectDefined : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 3)] public string ProjectOutcome { get; private set; }
        [DataMember(Order = 4)] public ProjectType Type { get; private set; }
        [DataMember(Order = 5)] public DateTime TimeUtc { get; private set; }
        
        ProjectDefined () {}
        public ProjectDefined (TrustedSystemId id, ProjectId projectId, string projectOutcome, ProjectType type, DateTime timeUtc)
        {
            Id = id;
            ProjectId = projectId;
            ProjectOutcome = projectOutcome;
            Type = type;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class DefineSingleActionProject : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public RequestId RequestId { get; private set; }
        [DataMember(Order = 3)] public StuffId StuffId { get; private set; }
        
        DefineSingleActionProject () {}
        public DefineSingleActionProject (TrustedSystemId id, RequestId requestId, StuffId stuffId)
        {
            Id = id;
            RequestId = requestId;
            StuffId = stuffId;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ChangeProjectType : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 3)] public ProjectType Type { get; private set; }
        
        ChangeProjectType () {}
        public ChangeProjectType (TrustedSystemId id, ProjectId projectId, ProjectType type)
        {
            Id = id;
            ProjectId = projectId;
            Type = type;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ProjectTypeChanged : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 3)] public ProjectType Type { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        ProjectTypeChanged () {}
        public ProjectTypeChanged (TrustedSystemId id, ProjectId projectId, ProjectType type, DateTime timeUtc)
        {
            Id = id;
            ProjectId = projectId;
            Type = type;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionAssignedToProject : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId NewProject { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        ActionAssignedToProject () {}
        public ActionAssignedToProject (TrustedSystemId id, ActionId actionId, ProjectId newProject, DateTime timeUtc)
        {
            Id = id;
            ActionId = actionId;
            NewProject = newProject;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionRemovedFromProject : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId OldProject { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        ActionRemovedFromProject () {}
        public ActionRemovedFromProject (TrustedSystemId id, ActionId actionId, ProjectId oldProject, DateTime timeUtc)
        {
            Id = id;
            ActionId = actionId;
            OldProject = oldProject;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionMovedToProject : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId OldProject { get; private set; }
        [DataMember(Order = 4)] public ProjectId NewProject { get; private set; }
        [DataMember(Order = 5)] public DateTime TimeUtc { get; private set; }
        
        ActionMovedToProject () {}
        public ActionMovedToProject (TrustedSystemId id, ActionId actionId, ProjectId oldProject, ProjectId newProject, DateTime timeUtc)
        {

            Id = id;
            ActionId = actionId;
            OldProject = oldProject;
            NewProject = newProject;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ArchiveAction : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        
        ArchiveAction () {}
        public ArchiveAction (TrustedSystemId id, ActionId actionId)
        {
            Id = id;
            ActionId = actionId;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionArchived : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        ActionArchived () {}
        public ActionArchived (TrustedSystemId id, ActionId actionId, ProjectId projectId, DateTime timeUtc)
        {
            Id = id;
            ActionId = actionId;
            ProjectId = projectId;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class CompleteAction : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        
        CompleteAction () {}
        public CompleteAction (TrustedSystemId id, ActionId actionId)
        {
            Id = id;
            ActionId = actionId;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionCompleted : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 4)] public string ActionOutcome { get; private set; }
        [DataMember(Order = 5)] public DateTime TimeUtc { get; private set; }
        
        ActionCompleted () {}
        public ActionCompleted (TrustedSystemId id, ActionId actionId, ProjectId projectId, string actionOutcome, DateTime timeUtc)
        {
            Id = id;
            ActionId = actionId;
            ProjectId = projectId;
            ActionOutcome = actionOutcome;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ChangeActionOutcome : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public string Outcome { get; private set; }
        
        ChangeActionOutcome () {}
        public ChangeActionOutcome (TrustedSystemId id, ActionId actionId, string outcome)
        {
            Id = id;
            ActionId = actionId;
            Outcome = outcome;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionOutcomeChanged : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 4)] public string ActionOutcome { get; private set; }
        [DataMember(Order = 5)] public DateTime TimeUtc { get; private set; }
        
        ActionOutcomeChanged () {}
        public ActionOutcomeChanged (TrustedSystemId id, ActionId actionId, ProjectId projectId, string actionOutcome, DateTime timeUtc)
        {
            Id = id;
            ActionId = actionId;
            ProjectId = projectId;
            ActionOutcome = actionOutcome;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ChangeProjectOutcome : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 3)] public string Outcome { get; private set; }
        
        ChangeProjectOutcome () {}
        public ChangeProjectOutcome (TrustedSystemId id, ProjectId projectId, string outcome)
        {
            Id = id;
            ProjectId = projectId;
            Outcome = outcome;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ProjectOutcomeChanged : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ProjectId ProjectId { get; private set; }
        [DataMember(Order = 3)] public string ProjectOutcome { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        ProjectOutcomeChanged () {}
        public ProjectOutcomeChanged (TrustedSystemId id, ProjectId projectId, string projectOutcome, DateTime timeUtc)
        {
            Id = id;
            ProjectId = projectId;
            ProjectOutcome = projectOutcome;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ChangeStuffDescription : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public StuffId StuffId { get; private set; }
        [DataMember(Order = 3)] public string NewDescriptionOfStuff { get; private set; }
        
        ChangeStuffDescription () {}
        public ChangeStuffDescription (TrustedSystemId id, StuffId stuffId, string newDescriptionOfStuff)
        {
            Id = id;
            StuffId = stuffId;
            NewDescriptionOfStuff = newDescriptionOfStuff;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class StuffDescriptionChanged : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public StuffId StuffId { get; private set; }
        [DataMember(Order = 3)] public string NewDescriptionOfStuff { get; private set; }
        [DataMember(Order = 4)] public DateTime TimeUtc { get; private set; }
        
        StuffDescriptionChanged () {}
        public StuffDescriptionChanged (TrustedSystemId id, StuffId stuffId, string newDescriptionOfStuff, DateTime timeUtc)
        {
            Id = id;
            StuffId = stuffId;
            NewDescriptionOfStuff = newDescriptionOfStuff;
            TimeUtc = timeUtc;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class DeferActionUntil : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime DeferUntil { get; private set; }
        
        DeferActionUntil () {}
        public DeferActionUntil (TrustedSystemId id, ActionId actionId, DateTime deferUntil)
        {
            Id = id;
            ActionId = actionId;
            DeferUntil = deferUntil;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionDeferredUntil : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime DeferUntil { get; private set; }
        
        ActionDeferredUntil () {}
        public ActionDeferredUntil (TrustedSystemId id, ActionId actionId, DateTime deferUntil)
        {
            Id = id;
            ActionId = actionId;
            DeferUntil = deferUntil;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionDeferDateShifted : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime OldDeferDate { get; private set; }
        [DataMember(Order = 4)] public DateTime NewDeferDate { get; private set; }
        
        ActionDeferDateShifted () {}
        public ActionDeferDateShifted (TrustedSystemId id, ActionId actionId, DateTime oldDeferDate, DateTime newDeferDate)
        {
            Id = id;
            ActionId = actionId;
            OldDeferDate = oldDeferDate;
            NewDeferDate = newDeferDate;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionIsNoLongerDeferred : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime OldDeferDate { get; private set; }
        
        ActionIsNoLongerDeferred () {}
        public ActionIsNoLongerDeferred (TrustedSystemId id, ActionId actionId, DateTime oldDeferDate)
        {
            Id = id;
            ActionId = actionId;
            OldDeferDate = oldDeferDate;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ProvideDueDateForAction : Command, ITrustedSystemCommand
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime NewDueDate { get; private set; }
        
        ProvideDueDateForAction () {}
        public ProvideDueDateForAction (TrustedSystemId id, ActionId actionId, DateTime newDueDate)
        {
            Id = id;
            ActionId = actionId;
            NewDueDate = newDueDate;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class DueDateAssignedToAction : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime NewDueDate { get; private set; }
        
        DueDateAssignedToAction () {}
        public DueDateAssignedToAction (TrustedSystemId id, ActionId actionId, DateTime newDueDate)
        {
            Id = id;
            ActionId = actionId;
            NewDueDate = newDueDate;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ActionDueDateMoved : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime OldDueDate { get; private set; }
        [DataMember(Order = 4)] public DateTime NewDueDate { get; private set; }
        
        ActionDueDateMoved () {}
        public ActionDueDateMoved (TrustedSystemId id, ActionId actionId, DateTime oldDueDate, DateTime newDueDate)
        {
            Id = id;
            ActionId = actionId;
            OldDueDate = oldDueDate;
            NewDueDate = newDueDate;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class DueDateRemovedFromAction : Event, ITrustedSystemEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        [DataMember(Order = 2)] public ActionId ActionId { get; private set; }
        [DataMember(Order = 3)] public DateTime OldDueDate { get; private set; }
        
        DueDateRemovedFromAction () {}
        public DueDateRemovedFromAction (TrustedSystemId id, ActionId actionId, DateTime oldDueDate)
        {
            Id = id;
            ActionId = actionId;
            OldDueDate = oldDueDate;
        }
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class InitClientProfileIfNeeded : Command
    {
    }
    [DataContract(Namespace = "BTW2/GTD")]
    public partial class ClientProfileSwitchedToTrustedSystem : Event, IClientProfileEvent
    {
        [DataMember(Order = 1)] public TrustedSystemId Id { get; private set; }
        
        ClientProfileSwitchedToTrustedSystem () {}
        public ClientProfileSwitchedToTrustedSystem (TrustedSystemId id)
        {
            Id = id;
        }
    }
    
    public interface IClientProfileApplicationService
    {
        void When(InitClientProfileIfNeeded c);
    }
    
    public interface IClientProfileState
    {
        void When(ClientProfileSwitchedToTrustedSystem e);
    }
    
    public interface ITrustedSystemApplicationService
    {
        void When(PutStuffInInbox c);
        void When(TrashStuff c);
        void When(DefineAction c);
        void When(DefineProject c);
        void When(DefineSingleActionProject c);
        void When(ChangeProjectType c);
        void When(ArchiveAction c);
        void When(CompleteAction c);
        void When(ChangeActionOutcome c);
        void When(ChangeProjectOutcome c);
        void When(ChangeStuffDescription c);
        void When(DeferActionUntil c);
        void When(ProvideDueDateForAction c);
    }
    
    public interface ITrustedSystemState
    {
        void When(TrustedSystemCreated e);
        void When(StuffPutInInbox e);
        void When(StuffTrashed e);
        void When(StuffArchived e);
        void When(ActionDefined e);
        void When(ProjectDefined e);
        void When(ProjectTypeChanged e);
        void When(ActionAssignedToProject e);
        void When(ActionRemovedFromProject e);
        void When(ActionMovedToProject e);
        void When(ActionArchived e);
        void When(ActionCompleted e);
        void When(ActionOutcomeChanged e);
        void When(ProjectOutcomeChanged e);
        void When(StuffDescriptionChanged e);
        void When(ActionDeferredUntil e);
        void When(ActionDeferDateShifted e);
        void When(ActionIsNoLongerDeferred e);
        void When(DueDateAssignedToAction e);
        void When(ActionDueDateMoved e);
        void When(DueDateRemovedFromAction e);
    }
    #endregion
}

