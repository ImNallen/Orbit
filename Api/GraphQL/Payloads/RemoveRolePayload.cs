namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for removeRole mutation.
/// </summary>
public sealed class RemoveRolePayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static RemoveRolePayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static RemoveRolePayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

