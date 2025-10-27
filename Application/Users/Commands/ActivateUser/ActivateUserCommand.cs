using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.ActivateUser;

/// <summary>
/// Command to activate a suspended user account.
/// </summary>
public sealed record ActivateUserCommand(Guid UserId) : IRequest<Result<ActivateUserResult, DomainError>>;

/// <summary>
/// Result of activating a user.
/// </summary>
public sealed record ActivateUserResult(string Message);

