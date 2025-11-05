using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryById;

/// <summary>
/// Query to get an inventory record by ID.
/// </summary>
public sealed record GetInventoryByIdQuery(Guid InventoryId) : IRequest<Result<InventoryDto, DomainError>>;

/// <summary>
/// Inventory data transfer object.
/// </summary>
public sealed record InventoryDto(
    Guid InventoryId,
    Guid ProductId,
    Guid LocationId,
    int Quantity,
    int ReservedQuantity,
    int AvailableQuantity,
    DateTime CreatedAt,
    DateTime UpdatedAt);

