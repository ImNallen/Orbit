using Application.Abstractions.Events;
using Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.EventHandlers;

/// <summary>
/// Handles UserRegisteredEvent for audit logging.
/// Note: Email verification is sent by the RegisterUserCommandHandler.
/// </summary>
public sealed class UserRegisteredEventHandler : INotificationHandler<DomainEventNotification<UserRegisteredEvent>>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<UserRegisteredEvent> notification, CancellationToken cancellationToken)
    {
        UserRegisteredEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "User {UserId} ({Email}) registered successfully: {FirstName} {LastName}",
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.FirstName,
            domainEvent.LastName);

        // In production, you might want to:
        // - Store this in an audit log table
        // - Send to analytics service
        // - Trigger welcome email workflow

        return Task.CompletedTask;
    }
}

