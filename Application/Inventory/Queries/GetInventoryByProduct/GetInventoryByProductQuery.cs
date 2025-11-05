using Application.Inventory.Queries.GetInventoryById;
using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryByProduct;

/// <summary>
/// Query to get all inventory records for a specific product across all locations.
/// </summary>
public sealed record GetInventoryByProductQuery(Guid ProductId) : IRequest<Result<GetInventoryByProductResult, DomainError>>;

/// <summary>
/// Result of get inventory by product query.
/// </summary>
public sealed record GetInventoryByProductResult(
    Guid ProductId,
    List<InventoryDto> Inventories,
    int TotalQuantity,
    int TotalReservedQuantity,
    int TotalAvailableQuantity);

