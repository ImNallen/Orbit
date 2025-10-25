namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for revokeAllSessions mutation.
/// </summary>
public sealed class RevokeAllSessionsPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public int RevokedCount { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static RevokeAllSessionsPayload Success(string message, int revokedCount) => new()
    {
        IsSuccess = true,
        Message = message,
        RevokedCount = revokedCount
    };

    public static RevokeAllSessionsPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        RevokedCount = 0,
        Errors = errors
    };
}

