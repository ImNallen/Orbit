namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for revokeSession mutation.
/// </summary>
public sealed class RevokeSessionPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static RevokeSessionPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static RevokeSessionPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

