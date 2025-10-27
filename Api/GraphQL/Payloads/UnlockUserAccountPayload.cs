namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for unlockUserAccount mutation.
/// </summary>
public sealed class UnlockUserAccountPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static UnlockUserAccountPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static UnlockUserAccountPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

