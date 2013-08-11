#region (c) 2012-2013 Copyright BeingTheWorst.com

// This project is sample code that is discussed on the
// "Being the Worst" podcast.
// Subscribe to our podcast feed at:
// http://beingtheworst.com/feed
// and follow us on twitter @beingtheworst
// for more details.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Gtd.Client.Models;
using Gtd.Shell.Filters;

namespace Gtd.Client
{
    #region This is the language of the UI expressed as Command and Event Messages...
    // These are the "words" the "UI component services"/controllers use to communicate.
    // These are UI events specific to the CONTEXT of this client application.
    // This Ui.cs is Similar to Messages.cs in our Gtd.PublishedLanguage project.
    // We are talking about "Inbox Displayed" and "DefineNewProject"
    // instead of just Windows Forms events like "Button Clicked, Text changed" etc.
    // We start to see the possibility of having a common UI event language 
    // that multiple UI (View) and/or platform implementations can "speak" in a common way.
    // WinForms, WPF, HTML Single Page Application (SPA), iOS, WinRT, etc.
    #endregion
    
    public static class UI
    {
        public abstract class NavigateCommand : Message{}

        public class DisplayInbox : NavigateCommand
        {
            public DisplayInbox() {}
        }

        public class DragStarted : Message
        {
            public readonly DragManager Manager;
            public DragStarted(DragManager manager)
            {
                
                Manager = manager;
            }
        }

        public class DragCompleted : Message
        {
            public readonly string RequestId;
            public DragCompleted(string requestId)
            {
                RequestId = requestId;
            }
        }

        public class InboxDisplayed : Message {}

        

        public class DisplayProject : NavigateCommand
        {
            public readonly ProjectId Id;

            public DisplayProject(ProjectId id)
            {
                Id = id;
            }
        }

        public class ProjectDisplayed : Message
        {
            public readonly ImmutableProjectInfo Project;


            public ProjectDisplayed(ImmutableProjectInfo project)
            {
                this.Project = project;
            }
        }

        public class CompleteActionClicked : Message
        {
            public readonly ActionId Id;

            public CompleteActionClicked(ActionId id)
            {
                Id = id;
            }
        }

        public class DragAction : Message
        {
            public readonly Guid RequestId;
            public readonly ImmutableAction Action;

            public DragAction(Guid requestId, ImmutableAction action)
            {
                RequestId = requestId;
                Action = action;
            }
        }

        public class ChangeActionOutcome : Message
        {
            public readonly ActionId ActionId;
            public readonly string Outcome;

            public ChangeActionOutcome(ActionId actionId, string outcome)
            {
                ActionId = actionId;
                Outcome = outcome;
            }
        }

        public class MoveActionsToProject : Message
        {
            public readonly IImmutableList<ActionId> Actions;
            public readonly ProjectId ToProject;

            public MoveActionsToProject(IImmutableList<ActionId> actions, ProjectId toProject)
            {
                Actions = actions;
                ToProject = toProject;
            }
        }

        public sealed class DefineNewProjectWizardCompleted : Message
        {
            public readonly string Outcome;

            public DefineNewProjectWizardCompleted(string outcome)
            {
                Outcome = outcome;
            }
        }

        public sealed class AddStuffClicked : Message {}

        public sealed class AddActionClicked : Message
        {
            public readonly ProjectId ProjectId;

            public AddActionClicked(ProjectId projectId)
            {
                ProjectId = projectId;
            }
        }

        public sealed class DefineProjectClicked : Message {}


        public sealed class AddStuffWizardCompleted : Message
        {
            public readonly string StuffDescription;

            public AddStuffWizardCompleted(string descriptionOfStuff)
            {
                StuffDescription = descriptionOfStuff;
            }
        }

        public sealed class DefineActionWizardCompleted : Message
        {
            public readonly ProjectId ProjectId;
            public readonly string Outcome;

            public DefineActionWizardCompleted(ProjectId projectId, string outcome)
            {
                ProjectId = projectId;
                Outcome = outcome;
            }
        }

        public sealed class MoveStuffToProjectClicked : Message
        {
            public readonly StuffId[] StuffToMove;
            public readonly ProjectId Project;

            public MoveStuffToProjectClicked(StuffId[] stuffToMove, ProjectId project)
            {
                StuffToMove = stuffToMove;
                Project = project;
            }
        }

        public sealed class TrashStuffClicked : Message
        {
            public readonly StuffId Id;

            public TrashStuffClicked(StuffId id)
            {
                Id = id;
            }
        }



        public sealed class ActionFilterChanged : Message
        {
            public readonly IFilterCriteria Criteria;

            public ActionFilterChanged(IFilterCriteria criteria)
            {
                Criteria = criteria;
            }
        }
    }

    public sealed class AppInit : Message { }

    public sealed class ProfileLoaded : Message
    {
        public readonly TrustedSystemId SystemId;

        public ProfileLoaded(TrustedSystemId systemId)
        {
            SystemId = systemId;
        }
    }

    public sealed class DragActions
    {
        public readonly string Request;
        public readonly ImmutableList<ImmutableAction> Actions;

        public static DragActions CreateRequest(IEnumerable<ImmutableAction> actions)
        {
            var requestId = Guid.NewGuid().ToString();
            return new DragActions(requestId, actions.ToImmutableList());
        }

        DragActions(string request, ImmutableList<ImmutableAction> actions)
        {
            Request = request;
            Actions = actions;
        }
    }

    public sealed class DragSubject<T>
    {
        public readonly string Request;
        public readonly T Subject;

        public DragSubject(string request, T subject)
        {
            Request = request;
            Subject = subject;
        }
    }
}