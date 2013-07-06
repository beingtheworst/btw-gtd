namespace Gtd.Client
{
    public static class Ui
    {

        public class DisplayInbox : Message
        {

        }
        public class InboxDisplayed : Message
        {

        }

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

        public class CompleteAction : Message
        {
            public readonly ActionId Id;

            public CompleteAction(ActionId id)
            {
                Id = id;
            }
        }

        public sealed class DefineNewProject : Message
        {
            public readonly string Outcome;

            public DefineNewProject(string outcome)
            {
                Outcome = outcome;
            }
        }

        public sealed class CaptureThoughtClicked : Message
        {

        }

        public sealed class DefineProjectClicked : Message
        {
            
        }


        public sealed class CaptureThought : Message
        {
            public readonly string Thought;

            public CaptureThought(string thought)
            {
                Thought = thought;
            }
        }

        public sealed class MoveThoughtsToProject : Message
        {
            public readonly ThoughtId[] Thoughts;
            public readonly ProjectId Project;

            public MoveThoughtsToProject(ThoughtId[] thoughts, ProjectId project)
            {
                Thoughts = thoughts;
                Project = project;
            }
        }

        public sealed class ArchiveThought : Message
        {
            public readonly ThoughtId Id;

            public ArchiveThought(ThoughtId id)
            {
                Id = id;
            }
        }



    }
}