namespace CrediFlow.Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
}

public abstract class DomainEventBase : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
