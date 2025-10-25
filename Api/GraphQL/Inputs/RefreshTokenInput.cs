namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for refreshing an access token.
/// </summary>
public sealed record RefreshTokenInput(
    string RefreshToken);

