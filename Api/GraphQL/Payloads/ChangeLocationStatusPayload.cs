namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for changeLocationStatus mutation.
/// </summary>
public sealed class ChangeLocationStatusPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static ChangeLocationStatusPayload Success(string message) =>
        new() { Message = message };

    public static ChangeLocationStatusPayload Failure(params LocationError[] errors) =>
        new() { Errors = errors };
}

