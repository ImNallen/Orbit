namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for assignLocationManager mutation.
/// </summary>
public sealed class AssignLocationManagerPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static AssignLocationManagerPayload Success(string message) => new()
    {
        Message = message
    };

    public static AssignLocationManagerPayload Failure(params LocationError[] errors) => new()
    {
        Errors = errors
    };
}

