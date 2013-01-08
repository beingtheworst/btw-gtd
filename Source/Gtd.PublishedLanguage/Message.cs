namespace Gtd
{
    public abstract class Message
    {
        
    }

    public abstract class Command : Message {}
    public abstract class Event : Message {}


    public interface ITenantCommand
    {
        TenantId Id { get; }
    }
    public interface ITenantEvent
    {
        TenantId Id { get; }
    }
}