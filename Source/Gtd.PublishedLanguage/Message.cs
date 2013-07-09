namespace Gtd
{
    public abstract class Message
    {
        
    }

    public abstract class Command : Message {}
    public abstract class Event : Message {}


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