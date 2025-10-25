namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for user login.
/// </summary>
public sealed record LoginInput(
    string Email,
    string Password);

