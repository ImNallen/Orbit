namespace Api.GraphQL.Payloads;

/// <summary>
/// Error type for location operations.
/// </summary>
public sealed record LocationError(string Code, string Message);

