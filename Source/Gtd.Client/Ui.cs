#region (c) 2012-2013 Copyright BeingTheWorst.com

// This project is sample code that is discussed on the
// "Being the Worst" podcast.
// Subscribe to our podcast feed at:
// http://beingtheworst.com/feed
// and follow us on twitter @beingtheworst
// for more details.

#endregion

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
        public class DisplayInbox : Message {}

        public class InboxDisplayed : Message {}

        public class DisplayProject : Message
        {
            public readonly ProjectId Id;

            public DisplayProject(ProjectId id)
            {
                Id = id;
            }
        }

        public class ProjectDisplayed : Message
        {
            public readonly ProjectId Id;

            public ProjectDisplayed(ProjectId id)
            {
                Id = id;
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



        public sealed class FilterChanged : Message
        {
            public readonly IFilterCriteria Criteria;

            public FilterChanged(IFilterCriteria criteria)
            {
                Criteria = criteria;
            }
        }
    }
}