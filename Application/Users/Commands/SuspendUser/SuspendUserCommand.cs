using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.SuspendUser;

/// <summary>
/// Command to suspend a user account.
/// </summary>
public sealed record SuspendUserCommand(Guid UserId) : IRequest<Result<SuspendUserResult, DomainError>>;

/// <summary>
/// Result of suspending a user.
/// </summary>
public sealed record SuspendUserResult(string Message);

