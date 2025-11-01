namespace Domain.Abstractions;

/// <summary>
/// Base record for domain events. Automatically generates Id and OccurredAtUtc.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
        Id = Guid.CreateVersion7();
        OccurredAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; init; }
    
    public DateTime OccurredAtUtc { get; init; }
}

