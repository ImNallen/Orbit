namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for deleteUser mutation.
/// </summary>
public sealed class DeleteUserPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static DeleteUserPayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static DeleteUserPayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

