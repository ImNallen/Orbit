namespace Api.GraphQL.Payloads;

/// <summary>
/// Error type for inventory operations.
/// </summary>
public sealed record InventoryError(string Code, string Message);

