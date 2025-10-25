namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for revokeSession mutation.
/// </summary>
public sealed record RevokeSessionInput(Guid SessionId);

