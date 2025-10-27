namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for updateUserProfile mutation.
/// </summary>
public sealed class UpdateUserProfilePayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public Guid? UserId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static UpdateUserProfilePayload Success(
        Guid userId,
        string firstName,
        string lastName,
        string message) => new()
    {
        IsSuccess = true,
        UserId = userId,
        FirstName = firstName,
        LastName = lastName,
        Message = message
    };

    public static UpdateUserProfilePayload Failure(params UserError[] errors) => new()
    {
        IsSuccess = false,
        Errors = errors
    };
}

