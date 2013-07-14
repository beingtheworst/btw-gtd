#region (c) 2012-2013 Copyright BeingTheWorst.com Podcast

// See being the worst podcast for more details

#endregion

using Gtd.Shell.Filters;

namespace Gtd.Client
{
    // this is the language of the UI
    // expressed as Command and Event Messages
    // this is how the "UI component services" communicate
    public static class Ui
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

        public sealed class DefineNewProjectWizardCompleted : Message
        {
            public readonly string Outcome;

            public DefineNewProjectWizardCompleted(string outcome)
            {
                Outcome = outcome;
            }
        }

        public sealed class CaptureThoughtClicked : Message {}

        public sealed class DefineProjectClicked : Message {}


        public sealed class CaptureThoughtWizardCompleted : Message
        {
            public readonly string Thought;

            public CaptureThoughtWizardCompleted(string thought)
            {
                Thought = thought;
            }
        }

        public sealed class MoveThoughtsToProjectClicked : Message
        {
            public readonly ThoughtId[] Thoughts;
            public readonly ProjectId Project;

            public MoveThoughtsToProjectClicked(ThoughtId[] thoughts, ProjectId project)
            {
                Thoughts = thoughts;
                Project = project;
            }
        }

        public sealed class ArchiveThoughtClicked : Message
        {
            public readonly ThoughtId Id;

            public ArchiveThoughtClicked(ThoughtId id)
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