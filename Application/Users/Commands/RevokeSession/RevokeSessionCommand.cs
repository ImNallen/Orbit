using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.RevokeSession;

/// <summary>
/// Command to revoke a specific session.
/// </summary>
public sealed record RevokeSessionCommand(
    Guid UserId,
    Guid SessionId) : IRequest<Result<RevokeSessionResult, DomainError>>;

