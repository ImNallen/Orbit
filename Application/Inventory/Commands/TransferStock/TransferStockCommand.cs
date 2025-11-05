using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Commands.TransferStock;

/// <summary>
/// Command to transfer stock from one location to another.
/// </summary>
public sealed record TransferStockCommand(
    Guid FromInventoryId,
    Guid ToInventoryId,
    int Quantity,
    string Reason) : IRequest<Result<TransferStockResult, DomainError>>;

/// <summary>
/// Result of stock transfer.
/// </summary>
public sealed record TransferStockResult(
    Guid FromInventoryId,
    Guid ToInventoryId,
    int Quantity,
    int FromNewQuantity,
    int ToNewQuantity);

