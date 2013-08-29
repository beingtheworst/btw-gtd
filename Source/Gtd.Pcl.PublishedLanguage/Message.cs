namespace Gtd
{
    public abstract class Message
    {
        // generic marker class for now that
        // all of our "Message" classes derive from
    }

    public abstract class Command : Message { }
    public abstract class Event : Message { }


    public interface ITrustedSystemCommand
    {
        TrustedSystemId Id { get; }
    }
    public interface ITrustedSystemEvent
    {
        TrustedSystemId Id { get; }
    }

    public interface IClientProfileEvent
    {

    }
}