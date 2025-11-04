namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for creating a product.
/// </summary>
public sealed record CreateProductInput(
    string Name,
    string Description,
    decimal Price,
    string Sku,
    int? InitialStock = null);

