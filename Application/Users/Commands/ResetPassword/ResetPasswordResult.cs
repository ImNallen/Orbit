using Domain.Abstractions;

namespace Application.Users.Commands.ResetPassword;

/// <summary>
/// Result of resetting a password.
/// </summary>
public sealed record ResetPasswordResult(
    bool IsSuccess,
    string Message,
    DomainError[] Errors);

