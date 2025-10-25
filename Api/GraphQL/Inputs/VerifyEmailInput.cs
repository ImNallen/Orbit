namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for email verification.
/// </summary>
public sealed record VerifyEmailInput(
    string Token);

