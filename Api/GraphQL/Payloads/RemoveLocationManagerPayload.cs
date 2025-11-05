namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for removeLocationManager mutation.
/// </summary>
public sealed class RemoveLocationManagerPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static RemoveLocationManagerPayload Success(string message) => new()
    {
        Message = message
    };

    public static RemoveLocationManagerPayload Failure(params LocationError[] errors) => new()
    {
        Errors = errors
    };
}

