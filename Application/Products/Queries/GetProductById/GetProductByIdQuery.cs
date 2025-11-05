using Domain.Abstractions;
using MediatR;

namespace Application.Products.Queries.GetProductById;

/// <summary>
/// Query to get a product by ID.
/// </summary>
public sealed record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductDto, DomainError>>;

/// <summary>
/// Product data transfer object.
/// </summary>
public sealed record ProductDto(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string Sku,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

