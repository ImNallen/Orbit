namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for switchLocationContext mutation.
/// </summary>
public sealed class SwitchLocationContextPayload
{
    public Guid? UserId { get; init; }
    public Guid? NewLocationContextId { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static SwitchLocationContextPayload Success(
        Guid userId,
        Guid newLocationContextId,
        string message) => new()
    {
        UserId = userId,
        NewLocationContextId = newLocationContextId,
        Message = message
    };

    public static SwitchLocationContextPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

