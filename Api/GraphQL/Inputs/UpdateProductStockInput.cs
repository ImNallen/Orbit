namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for updating product stock.
/// </summary>
public sealed record UpdateProductStockInput(
    Guid ProductId,
    int Quantity);

