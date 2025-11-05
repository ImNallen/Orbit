namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for transferring stock between locations.
/// </summary>
public sealed record TransferStockInput(
    Guid FromInventoryId,
    Guid ToInventoryId,
    int Quantity,
    string? Reason = null);

