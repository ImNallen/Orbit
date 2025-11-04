using Domain.Abstractions;
using MediatR;

namespace Application.Products.Commands.UpdateProductStock;

/// <summary>
/// Command to update product stock quantity.
/// </summary>
public sealed record UpdateProductStockCommand(
    Guid ProductId,
    int Quantity) : IRequest<Result<UpdateProductStockResult, DomainError>>;

/// <summary>
/// Result of updating product stock.
/// </summary>
public sealed record UpdateProductStockResult(string Message, int NewStockQuantity);

