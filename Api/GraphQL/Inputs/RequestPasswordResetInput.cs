namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for requesting a password reset.
/// </summary>
public sealed record RequestPasswordResetInput(
    string Email);

