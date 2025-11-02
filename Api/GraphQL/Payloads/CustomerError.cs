namespace Api.GraphQL.Payloads;

/// <summary>
/// Represents an error that occurred during a customer operation.
/// </summary>
public sealed record CustomerError(string Code, string Message);

