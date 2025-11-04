using Domain.Abstractions;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

/// <summary>
/// Command to delete a product.
/// </summary>
public sealed record DeleteProductCommand(Guid ProductId) : IRequest<Result<DeleteProductResult, DomainError>>;

/// <summary>
/// Result of deleting a product.
/// </summary>
public sealed record DeleteProductResult(string Message);

