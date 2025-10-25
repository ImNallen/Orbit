using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.ResetPassword;

/// <summary>
/// Command to reset a user's password using a reset token.
/// </summary>
public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword) : IRequest<Result<ResetPasswordResult, DomainError>>;

