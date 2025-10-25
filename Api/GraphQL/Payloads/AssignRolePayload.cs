namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for assignRole mutation.
/// </summary>
public sealed class AssignRolePayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static AssignRolePayload Success(string message) => new()
    {
        IsSuccess = true,
        Message = message
    };

    public static AssignRolePayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

