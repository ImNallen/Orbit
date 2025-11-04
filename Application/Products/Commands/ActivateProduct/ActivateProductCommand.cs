using Domain.Abstractions;
using MediatR;

namespace Application.Products.Commands.ActivateProduct;

/// <summary>
/// Command to activate a product.
/// </summary>
public sealed record ActivateProductCommand(Guid ProductId) : IRequest<Result<ActivateProductResult, DomainError>>;

/// <summary>
/// Result of activating a product.
/// </summary>
public sealed record ActivateProductResult(string Message);

