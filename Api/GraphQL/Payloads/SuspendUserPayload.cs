namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for suspendUser mutation.
/// </summary>
public sealed class SuspendUserPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static SuspendUserPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static SuspendUserPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

