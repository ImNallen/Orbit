using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for createLocation mutation.
/// </summary>
public sealed class CreateLocationPayload
{
    public LocationType? Location { get; init; }
    public IReadOnlyList<LocationError> Errors { get; init; } = Array.Empty<LocationError>();

    public static CreateLocationPayload Success(LocationType location) =>
        new() { Location = location };

    public static CreateLocationPayload Failure(params LocationError[] errors) =>
        new() { Errors = errors };
}

