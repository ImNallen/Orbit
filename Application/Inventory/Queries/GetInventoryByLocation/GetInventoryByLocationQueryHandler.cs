using Application.Inventory.Queries.GetInventoryById;
using Domain.Abstractions;
using Domain.Inventory;
using Domain.Locations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Queries.GetInventoryByLocation;

/// <summary>
/// Handler for GetInventoryByLocationQuery.
/// </summary>
public sealed class GetInventoryByLocationQueryHandler
    : IRequestHandler<GetInventoryByLocationQuery, Result<GetInventoryByLocationResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetInventoryByLocationQueryHandler> _logger;

    public GetInventoryByLocationQueryHandler(
        IInventoryRepository inventoryRepository,
        ILocationRepository locationRepository,
        ILogger<GetInventoryByLocationQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<GetInventoryByLocationResult, DomainError>> Handle(
        GetInventoryByLocationQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting inventory for location {LocationId}", query.LocationId);

        // Verify location exists
        Location? location = await _locationRepository.GetByIdAsync(query.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", query.LocationId);
            return Result<GetInventoryByLocationResult, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        List<Domain.Inventory.Inventory> inventories = await _inventoryRepository.GetByLocationIdAsync(
            query.LocationId,
            cancellationToken);

        var inventoryDtos = inventories.Select(i => new InventoryDto(
            i.Id,
            i.ProductId,
            i.LocationId,
            i.Quantity,
            i.ReservedQuantity,
            i.AvailableQuantity,
            i.CreatedAt,
            i.UpdatedAt)).ToList();

        int totalProducts = inventories.Count;
        int totalQuantity = inventories.Sum(i => i.Quantity);

        GetInventoryByLocationResult result = new(
            query.LocationId,
            inventoryDtos,
            totalProducts,
            totalQuantity);

        return Result<GetInventoryByLocationResult, DomainError>.Success(result);
    }
}

