namespace Api.GraphQL.Payloads;

/// <summary>
/// Represents an error that occurred during a product operation.
/// </summary>
public sealed record ProductError(string Code, string Message);

