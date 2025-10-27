using Domain.Abstractions;
using MediatR;

namespace Infrastructure.Events;

/// <summary>
/// Wrapper that adapts domain events to MediatR notifications.
/// This keeps the Domain layer pure and free from infrastructure dependencies.
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event being wrapped.</typeparam>
public sealed class DomainEventNotification<TDomainEvent> : INotification
    where TDomainEvent : IDomainEvent
{
    public DomainEventNotification(TDomainEvent domainEvent)
    {
        DomainEvent = domainEvent;
    }

    public TDomainEvent DomainEvent { get; }
}

