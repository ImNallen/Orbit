namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for deleting a product.
/// </summary>
public sealed record DeleteProductInput(Guid ProductId);

