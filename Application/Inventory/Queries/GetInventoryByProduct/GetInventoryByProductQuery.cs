using Application.Common.Enums;
using Application.Inventory.Queries.GetInventoryById;
using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Queries.GetInventoryByProduct;

/// <summary>
/// Query to get inventory records for a specific product.
/// By default, filters to the user's current location context.
/// </summary>
public sealed record GetInventoryByProductQuery(
    Guid ProductId,
    LocationFilterMode FilterMode = LocationFilterMode.CurrentContext,
    IEnumerable<Guid>? SpecificLocationIds = null) : IRequest<Result<GetInventoryByProductResult, DomainError>>;

/// <summary>
/// Result of get inventory by product query.
/// </summary>
public sealed record GetInventoryByProductResult(
    Guid ProductId,
    List<InventoryDto> Inventories,
    int TotalQuantity,
    int TotalReservedQuantity,
    int TotalAvailableQuantity);

