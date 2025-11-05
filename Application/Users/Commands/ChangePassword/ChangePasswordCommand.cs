using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.ChangePassword;

/// <summary>
/// Command to change a user's password (for authenticated users).
/// Different from ResetPassword which uses a token for forgotten passwords.
/// </summary>
public sealed record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword) : IRequest<Result<ChangePasswordResult, DomainError>>;

/// <summary>
/// Result of password change.
/// </summary>
public sealed record ChangePasswordResult(string Message);

