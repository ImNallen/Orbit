namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for activateUser mutation.
/// </summary>
public sealed class ActivateUserPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static ActivateUserPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static ActivateUserPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

