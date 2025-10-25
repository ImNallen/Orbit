using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.RequestPasswordReset;

/// <summary>
/// Command to request a password reset.
/// </summary>
public sealed record RequestPasswordResetCommand(string Email) : IRequest<Result<RequestPasswordResetResult, DomainError>>;

