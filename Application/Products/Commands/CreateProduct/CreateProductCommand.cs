using Domain.Abstractions;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

/// <summary>
/// Command to create a new product.
/// </summary>
public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    string Sku,
    int InitialStock = 0) : IRequest<Result<CreateProductResult, DomainError>>;

/// <summary>
/// Result of product creation.
/// </summary>
public sealed record CreateProductResult(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price);

