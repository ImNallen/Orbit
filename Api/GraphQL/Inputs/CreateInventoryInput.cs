namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for creating an inventory record.
/// </summary>
public sealed record CreateInventoryInput(
    Guid ProductId,
    Guid LocationId,
    int InitialQuantity = 0);

