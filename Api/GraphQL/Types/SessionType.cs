namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for session information.
/// </summary>
public sealed class SessionType
{
    public Guid SessionId { get; init; }
    public string IpAddress { get; init; } = string.Empty;
    public string UserAgent { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public DateTime LastAccessedAt { get; init; }
    public bool IsCurrentSession { get; init; }
}

