using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Commands.ReleaseReservation;

/// <summary>
/// Command to release a stock reservation (e.g., when an order is cancelled).
/// </summary>
public sealed record ReleaseReservationCommand(
    Guid InventoryId,
    int Quantity) : IRequest<Result<ReleaseReservationResult, DomainError>>;

/// <summary>
/// Result of reservation release.
/// </summary>
public sealed record ReleaseReservationResult(
    Guid InventoryId,
    int ReservedQuantity,
    int AvailableQuantity);

