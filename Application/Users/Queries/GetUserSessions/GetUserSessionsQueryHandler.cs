using Domain.Abstractions;
using Domain.Users.Session;
using MediatR;

namespace Application.Users.Queries.GetUserSessions;

/// <summary>
/// Handler for GetUserSessionsQuery.
/// </summary>
public sealed class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, Result<GetUserSessionsResult, DomainError>>
{
    private readonly ISessionRepository _sessionRepository;

    public GetUserSessionsQueryHandler(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Result<GetUserSessionsResult, DomainError>> Handle(
        GetUserSessionsQuery query,
        CancellationToken cancellationToken)
    {
        // Get all active sessions for the user
        List<Session> sessions = await _sessionRepository.GetActiveSessionsByUserIdAsync(query.UserId, cancellationToken);

        // Map to DTOs
        var sessionDtos = sessions.Select(s => new SessionDto(
            s.Id,
            s.IpAddress,
            s.UserAgent,
            s.CreatedAt,
            s.ExpiresAt,
            s.LastAccessedAt,
            false // We'll set this in the GraphQL layer based on the current session
        )).ToList();

        return Result<GetUserSessionsResult, DomainError>.Success(
            new GetUserSessionsResult(sessionDtos));
    }
}

