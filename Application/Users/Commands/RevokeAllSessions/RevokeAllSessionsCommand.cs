using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.RevokeAllSessions;

/// <summary>
/// Command to revoke all sessions for a user except the current one.
/// </summary>
public sealed record RevokeAllSessionsCommand(
    Guid UserId,
    Guid? CurrentSessionId = null) : IRequest<Result<RevokeAllSessionsResult, DomainError>>;

