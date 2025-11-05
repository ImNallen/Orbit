using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Commands.CommitReservation;

/// <summary>
/// Command to commit a stock reservation (e.g., when an order is fulfilled).
/// This reduces both the reserved quantity and the total quantity.
/// </summary>
public sealed record CommitReservationCommand(
    Guid InventoryId,
    int Quantity) : IRequest<Result<CommitReservationResult, DomainError>>;

/// <summary>
/// Result of committing a reservation.
/// </summary>
public sealed record CommitReservationResult(
    Guid InventoryId,
    int NewQuantity,
    int NewReservedQuantity,
    int AvailableQuantity);

