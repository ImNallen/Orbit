namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for updating a product.
/// </summary>
public sealed record UpdateProductInput(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price);

