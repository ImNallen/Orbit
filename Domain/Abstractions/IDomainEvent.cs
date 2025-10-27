namespace Domain.Abstractions;

/// <summary>
/// Base interface for domain events.
/// </summary>
public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredAtUtc { get; }
}
