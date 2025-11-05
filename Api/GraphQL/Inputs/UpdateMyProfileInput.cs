namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for authenticated users to update their own profile.
/// </summary>
public sealed record UpdateMyProfileInput(
    string FirstName,
    string LastName);

