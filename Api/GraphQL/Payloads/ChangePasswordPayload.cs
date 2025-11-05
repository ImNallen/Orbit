namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for changePassword mutation.
/// </summary>
public sealed class ChangePasswordPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static ChangePasswordPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static ChangePasswordPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

