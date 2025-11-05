namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for unassignUserFromLocation mutation.
/// </summary>
public sealed class UnassignUserFromLocationPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static UnassignUserFromLocationPayload Success(string message) => new()
    {
        Message = message
    };

    public static UnassignUserFromLocationPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

