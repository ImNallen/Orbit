namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for user registration.
/// </summary>
public sealed record RegisterUserInput(
    string Email,
    string Password,
    string FirstName,
    string LastName);

