using Domain.Abstractions;
using MediatR;

namespace Application.Users.Queries.GetUserSessions;

/// <summary>
/// Query to get all active sessions for the current user.
/// </summary>
public sealed record GetUserSessionsQuery(Guid UserId) : IRequest<Result<GetUserSessionsResult, DomainError>>;

