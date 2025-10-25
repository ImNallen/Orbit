namespace Application.Users.Queries.GetUserSessions;

/// <summary>
/// Result for GetUserSessionsQuery.
/// </summary>
public sealed record GetUserSessionsResult(IReadOnlyList<SessionDto> Sessions);

/// <summary>
/// DTO for session information.
/// </summary>
public sealed record SessionDto(
    Guid SessionId,
    string IpAddress,
    string UserAgent,
    DateTime CreatedAt,
    DateTime ExpiresAt,
    DateTime LastAccessedAt,
    bool IsCurrentSession);

