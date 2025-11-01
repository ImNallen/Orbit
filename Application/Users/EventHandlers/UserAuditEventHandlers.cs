using Application.Abstractions.Events;
using Domain.Role.Events;
using Domain.Users.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.EventHandlers;

/// <summary>
/// Handles UserLoggedInEvent for audit logging.
/// </summary>
public sealed class UserLoggedInEventHandler : INotificationHandler<DomainEventNotification<UserLoggedInEvent>>
{
    private readonly ILogger<UserLoggedInEventHandler> _logger;

    public UserLoggedInEventHandler(ILogger<UserLoggedInEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<UserLoggedInEvent> notification, CancellationToken cancellationToken)
    {
        UserLoggedInEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "User {UserId} logged in successfully from IP {IpAddress}. Session: {SessionId}",
            domainEvent.UserId,
            domainEvent.IpAddress,
            domainEvent.SessionId);

        // In production, you might want to:
        // - Store this in an audit log table
        // - Send to a SIEM system
        // - Trigger fraud detection if suspicious

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles LoginFailedEvent for audit logging and security monitoring.
/// </summary>
public sealed class LoginFailedEventHandler : INotificationHandler<DomainEventNotification<LoginFailedEvent>>
{
    private readonly ILogger<LoginFailedEventHandler> _logger;

    public LoginFailedEventHandler(ILogger<LoginFailedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<LoginFailedEvent> notification, CancellationToken cancellationToken)
    {
        LoginFailedEvent domainEvent = notification.DomainEvent;

        _logger.LogWarning(
            "Failed login attempt for user {UserId} ({Email}) from IP {IpAddress}. Attempt #{FailedAttemptCount}",
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.IpAddress,
            domainEvent.FailedAttemptCount);

        // In production, you might want to:
        // - Store this in an audit log table
        // - Trigger alerts if too many failed attempts from same IP
        // - Implement IP-based rate limiting

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles AccountLockedEvent for audit logging and notifications.
/// </summary>
public sealed class AccountLockedEventHandler : INotificationHandler<DomainEventNotification<AccountLockedEvent>>
{
    private readonly ILogger<AccountLockedEventHandler> _logger;

    public AccountLockedEventHandler(ILogger<AccountLockedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<AccountLockedEvent> notification, CancellationToken cancellationToken)
    {
        AccountLockedEvent domainEvent = notification.DomainEvent;

        _logger.LogWarning(
            "User account {UserId} has been locked due to too many failed login attempts",
            domainEvent.UserId);

        // In production, you might want to:
        // - Send email notification to user
        // - Store in audit log
        // - Trigger security alerts

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles AccountUnlockedEvent for audit logging.
/// </summary>
public sealed class AccountUnlockedEventHandler : INotificationHandler<DomainEventNotification<AccountUnlockedEvent>>
{
    private readonly ILogger<AccountUnlockedEventHandler> _logger;

    public AccountUnlockedEventHandler(ILogger<AccountUnlockedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<AccountUnlockedEvent> notification, CancellationToken cancellationToken)
    {
        AccountUnlockedEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "User account {UserId} has been unlocked",
            domainEvent.UserId);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles UserEmailVerifiedEvent for audit logging.
/// </summary>
public sealed class UserEmailVerifiedEventHandler : INotificationHandler<DomainEventNotification<UserEmailVerifiedEvent>>
{
    private readonly ILogger<UserEmailVerifiedEventHandler> _logger;

    public UserEmailVerifiedEventHandler(ILogger<UserEmailVerifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<UserEmailVerifiedEvent> notification, CancellationToken cancellationToken)
    {
        UserEmailVerifiedEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Email verified for user {UserId} ({Email})",
            domainEvent.UserId,
            domainEvent.Email);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles PasswordChangedEvent for audit logging and security notifications.
/// </summary>
public sealed class PasswordChangedEventHandler : INotificationHandler<DomainEventNotification<PasswordChangedEvent>>
{
    private readonly ILogger<PasswordChangedEventHandler> _logger;

    public PasswordChangedEventHandler(ILogger<PasswordChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<PasswordChangedEvent> notification, CancellationToken cancellationToken)
    {
        PasswordChangedEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Password changed for user {UserId}",
            domainEvent.UserId);

        // In production, you might want to:
        // - Send email notification to user
        // - Revoke all existing sessions
        // - Store in audit log

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles RoleChangedEvent for audit logging.
/// </summary>
public sealed class RoleChangedEventHandler : INotificationHandler<DomainEventNotification<RoleChangedEvent>>
{
    private readonly ILogger<RoleChangedEventHandler> _logger;

    public RoleChangedEventHandler(ILogger<RoleChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<RoleChangedEvent> notification, CancellationToken cancellationToken)
    {
        RoleChangedEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Role changed for user {UserId}: {OldRoleId} -> {NewRoleId} ({NewRoleName})",
            domainEvent.UserId,
            domainEvent.OldRoleId,
            domainEvent.NewRoleId,
            domainEvent.NewRoleName);

        // In production, you might want to:
        // - Store in audit log
        // - Send notification to user
        // - Trigger compliance workflows

        return Task.CompletedTask;
    }
}

/// <summary>
/// Handles UserLoggedOutEvent for audit logging.
/// </summary>
public sealed class UserLoggedOutEventHandler : INotificationHandler<DomainEventNotification<UserLoggedOutEvent>>
{
    private readonly ILogger<UserLoggedOutEventHandler> _logger;

    public UserLoggedOutEventHandler(ILogger<UserLoggedOutEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(DomainEventNotification<UserLoggedOutEvent> notification, CancellationToken cancellationToken)
    {
        UserLoggedOutEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "User {UserId} logged out. Session: {SessionId}",
            domainEvent.UserId,
            domainEvent.SessionId);

        return Task.CompletedTask;
    }
}

