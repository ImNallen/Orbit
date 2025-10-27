using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.UnlockUserAccount;

/// <summary>
/// Command to manually unlock a locked user account.
/// </summary>
public sealed record UnlockUserAccountCommand(Guid UserId) : IRequest<Result<UnlockUserAccountResult, DomainError>>;

/// <summary>
/// Result of unlocking a user account.
/// </summary>
public sealed record UnlockUserAccountResult(string Message);

