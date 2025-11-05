namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for removeLocationOwner mutation.
/// </summary>
public sealed class RemoveLocationOwnerPayload
{
    public string? Message { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static RemoveLocationOwnerPayload Success(string message) => new()
    {
        Message = message
    };

    public static RemoveLocationOwnerPayload Failure(params LocationError[] errors) => new()
    {
        Errors = errors
    };
}

