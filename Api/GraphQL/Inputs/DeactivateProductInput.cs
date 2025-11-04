namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for deactivating a product.
/// </summary>
public sealed record DeactivateProductInput(Guid ProductId);

