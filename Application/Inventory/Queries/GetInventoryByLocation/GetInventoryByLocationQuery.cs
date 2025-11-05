using Application.Inventory.Queries.GetInventoryById;
using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryByLocation;

/// <summary>
/// Query to get all inventory records for a specific location.
/// </summary>
public sealed record GetInventoryByLocationQuery(Guid LocationId) : IRequest<Result<GetInventoryByLocationResult, DomainError>>;

/// <summary>
/// Result of get inventory by location query.
/// </summary>
public sealed record GetInventoryByLocationResult(
    Guid LocationId,
    List<InventoryDto> Inventories,
    int TotalProducts,
    int TotalQuantity);

