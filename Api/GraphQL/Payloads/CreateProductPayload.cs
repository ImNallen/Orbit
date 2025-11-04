using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for createProduct mutation.
/// </summary>
public sealed class CreateProductPayload
{
    public ProductType? Product { get; init; }
    public IReadOnlyList<ProductError> Errors { get; init; } = Array.Empty<ProductError>();

    public static CreateProductPayload Success(ProductType product) =>
        new() { Product = product };

    public static CreateProductPayload Failure(params ProductError[] errors) =>
        new() { Errors = errors };
}

