namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for setPrimaryLocation mutation.
/// </summary>
public sealed class SetPrimaryLocationPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static SetPrimaryLocationPayload Success(string message) => new()
    {
        Message = message
    };

    public static SetPrimaryLocationPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

