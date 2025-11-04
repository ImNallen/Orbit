using Domain.Abstractions;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

/// <summary>
/// Command to update product information.
/// </summary>
public sealed record UpdateProductCommand(
    Guid ProductId,
    string Name,
    string Description,
    decimal Price) : IRequest<Result<UpdateProductResult, DomainError>>;

/// <summary>
/// Result of updating a product.
/// </summary>
public sealed record UpdateProductResult(string Message);

