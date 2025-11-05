namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for assignUserToLocation mutation.
/// </summary>
public sealed class AssignUserToLocationPayload
{
    public Guid? AssignmentId { get; init; }
    public Guid? UserId { get; init; }
    public Guid? LocationId { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static AssignUserToLocationPayload Success(
        Guid assignmentId,
        Guid userId,
        Guid locationId,
        string message) => new()
    {
        AssignmentId = assignmentId,
        UserId = userId,
        LocationId = locationId,
        Message = message
    };

    public static AssignUserToLocationPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

