using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Commands.ReserveStock;

/// <summary>
/// Command to reserve stock for an order or allocation.
/// </summary>
public sealed record ReserveStockCommand(
    Guid InventoryId,
    int Quantity) : IRequest<Result<ReserveStockResult, DomainError>>;

/// <summary>
/// Result of stock reservation.
/// </summary>
public sealed record ReserveStockResult(
    Guid InventoryId,
    int ReservedQuantity,
    int AvailableQuantity);

