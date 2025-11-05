namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for updateLocation mutation.
/// </summary>
public sealed class UpdateLocationPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static UpdateLocationPayload Success(string message) =>
        new() { Message = message };

    public static UpdateLocationPayload Failure(params LocationError[] errors) =>
        new() { Errors = errors };
}

