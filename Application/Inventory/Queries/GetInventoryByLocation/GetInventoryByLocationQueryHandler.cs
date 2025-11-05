using Application.Inventory.Queries.GetInventoryById;
using Application.Services;
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
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetInventoryByLocationQueryHandler> _logger;

    public GetInventoryByLocationQueryHandler(
        IInventoryRepository inventoryRepository,
        ILocationRepository locationRepository,
        ICurrentUserService currentUserService,
        ILogger<GetInventoryByLocationQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _locationRepository = locationRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<GetInventoryByLocationResult, DomainError>> Handle(
        GetInventoryByLocationQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting inventory for location {LocationId}", query.LocationId);

        // 1. Verify location exists
        Location? location = await _locationRepository.GetByIdAsync(query.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", query.LocationId);
            return Result<GetInventoryByLocationResult, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        // 2. Check if user has access to this location
        IEnumerable<Guid> accessibleLocationIds = await _currentUserService.GetAccessibleLocationIdsAsync(
            cancellationToken);

        if (!accessibleLocationIds.Contains(query.LocationId))
        {
            _logger.LogWarning(
                "User {UserId} does not have access to location {LocationId}",
                _currentUserService.GetUserId(), query.LocationId);
            return Result<GetInventoryByLocationResult, DomainError>.Failure(InventoryErrors.AccessDenied);
        }

        // 3. Get inventory for the location
        List<Domain.Inventory.Inventory> inventories = await _inventoryRepository.GetByLocationIdAsync(
            query.LocationId,
            cancellationToken);

        // 4. Map to DTOs
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

