using Application.Abstractions.Email;
using Application.Abstractions.Events;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.EventHandlers;

/// <summary>
/// Handles PasswordResetRequestedEvent by sending password reset email.
/// </summary>
public sealed class PasswordResetRequestedEventHandler : INotificationHandler<DomainEventNotification<PasswordResetRequestedEvent>>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<PasswordResetRequestedEventHandler> _logger;

    public PasswordResetRequestedEventHandler(
        IEmailService emailService,
        ILogger<PasswordResetRequestedEventHandler> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<PasswordResetRequestedEvent> notification, CancellationToken cancellationToken)
    {
        PasswordResetRequestedEvent domainEvent = notification.DomainEvent;

        _logger.LogInformation(
            "Handling PasswordResetRequestedEvent for user {UserId} ({Email})",
            domainEvent.UserId,
            domainEvent.Email);

        try
        {
            // Send password reset email
            // Note: We don't have the user's first name in the event, so we'll pass empty string
            // In production, you might want to include the name in the event or fetch it from the repository
            await _emailService.SendPasswordResetAsync(
                domainEvent.Email,
                string.Empty, // firstName - not available in the event
                domainEvent.ResetToken,
                cancellationToken);

            _logger.LogInformation(
                "Password reset email sent successfully to {Email}",
                domainEvent.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send password reset email to {Email}",
                domainEvent.Email);

            // Don't throw - we don't want to fail the password reset request if email sending fails
            // In production, you might want to queue this for retry
        }
    }
}

