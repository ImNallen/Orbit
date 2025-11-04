using Domain.Abstractions;
using MediatR;

namespace Application.Products.Commands.DeactivateProduct;

/// <summary>
/// Command to deactivate a product.
/// </summary>
public sealed record DeactivateProductCommand(Guid ProductId) : IRequest<Result<DeactivateProductResult, DomainError>>;

/// <summary>
/// Result of deactivating a product.
/// </summary>
public sealed record DeactivateProductResult(string Message);

