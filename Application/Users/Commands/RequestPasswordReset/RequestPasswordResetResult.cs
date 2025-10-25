using Domain.Abstractions;

namespace Application.Users.Commands.RequestPasswordReset;

/// <summary>
/// Result of requesting a password reset.
/// </summary>
public sealed record RequestPasswordResetResult(
    bool IsSuccess,
    string Message,
    DomainError[] Errors);

