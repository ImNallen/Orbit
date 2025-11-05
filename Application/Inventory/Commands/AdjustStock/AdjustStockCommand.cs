using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Commands.AdjustStock;

/// <summary>
/// Command to adjust stock quantity for an inventory record.
/// </summary>
public sealed record AdjustStockCommand(
    Guid InventoryId,
    int Adjustment,
    string Reason) : IRequest<Result<AdjustStockResult, DomainError>>;

/// <summary>
/// Result of stock adjustment.
/// </summary>
public sealed record AdjustStockResult(
    Guid InventoryId,
    int NewQuantity,
    int AvailableQuantity);

