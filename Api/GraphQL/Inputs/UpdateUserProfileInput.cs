namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for updateUserProfile mutation.
/// </summary>
public sealed record UpdateUserProfileInput(
    Guid UserId,
    string FirstName,
    string LastName);

