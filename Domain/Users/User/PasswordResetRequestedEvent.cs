using Domain.Abstractions;

namespace Domain.Users.User;

/// <summary>
/// Event raised when a user requests a password reset.
/// </summary>
public sealed record PasswordResetRequestedEvent(
    Guid UserId,
    string Email,
    string ResetToken) : DomainEvent;

