namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for assignLocationOwner mutation.
/// </summary>
public sealed class AssignLocationOwnerPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static AssignLocationOwnerPayload Success(string message) => new()
    {
        Message = message
    };

    public static AssignLocationOwnerPayload Failure(params LocationError[] errors) => new()
    {
        Errors = errors
    };
}

