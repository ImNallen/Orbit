using Application.Inventory.Queries.GetInventoryById;
using Application.Services;
using Domain.Abstractions;
using Domain.Inventory;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Queries.GetInventoryByProduct;

/// <summary>
/// Handler for GetInventoryByProductQuery.
/// </summary>
public sealed class GetInventoryByProductQueryHandler
    : IRequestHandler<GetInventoryByProductQuery, Result<GetInventoryByProductResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetInventoryByProductQueryHandler> _logger;

    public GetInventoryByProductQueryHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService,
        ILogger<GetInventoryByProductQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<GetInventoryByProductResult, DomainError>> Handle(
        GetInventoryByProductQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting inventory for product {ProductId} with filter mode {FilterMode}",
            query.ProductId,
            query.FilterMode);

        // 1. Verify product exists
        Product? product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", query.ProductId);
            return Result<GetInventoryByProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Determine which locations to filter by based on filter mode
        IEnumerable<Guid> targetLocationIds;

        switch (query.FilterMode)
        {
            case Application.Common.Enums.LocationFilterMode.CurrentContext:
                // Get user's current location context
                Guid? currentLocationId = await _currentUserService.GetCurrentLocationContextIdAsync(
                    cancellationToken);

                if (currentLocationId is null)
                {
                    _logger.LogWarning("User has no current location context set");
                    return Result<GetInventoryByProductResult, DomainError>.Failure(
                        InventoryErrors.NoLocationContextSet);
                }

                targetLocationIds = new[] { currentLocationId.Value };
                _logger.LogDebug("Filtering to current context location: {LocationId}", currentLocationId);
                break;

            case Application.Common.Enums.LocationFilterMode.AllAssigned:
                // Get all accessible locations
                targetLocationIds = await _currentUserService.GetAccessibleLocationIdsAsync(
                    cancellationToken);
                _logger.LogDebug("Filtering to all {Count} assigned locations", targetLocationIds.Count());
                break;

            case Application.Common.Enums.LocationFilterMode.Specific:
                // Use specific location IDs provided in query
                if (query.SpecificLocationIds is null || !query.SpecificLocationIds.Any())
                {
                    _logger.LogWarning("Specific filter mode requested but no location IDs provided");
                    return Result<GetInventoryByProductResult, DomainError>.Failure(
                        InventoryErrors.NoLocationIdsProvided);
                }

                // Verify user has access to all specified locations
                IEnumerable<Guid> accessibleLocationIds = await _currentUserService.GetAccessibleLocationIdsAsync(
                    cancellationToken);

                var unauthorizedLocations = query.SpecificLocationIds
                    .Where(id => !accessibleLocationIds.Contains(id))
                    .ToList();

                if (unauthorizedLocations.Any())
                {
                    _logger.LogWarning(
                        "User does not have access to {Count} requested locations",
                        unauthorizedLocations.Count);
                    return Result<GetInventoryByProductResult, DomainError>.Failure(
                        InventoryErrors.AccessDenied);
                }

                targetLocationIds = query.SpecificLocationIds;
                _logger.LogDebug("Filtering to {Count} specific locations", targetLocationIds.Count());
                break;

            default:
                _logger.LogError("Unknown filter mode: {FilterMode}", query.FilterMode);
                return Result<GetInventoryByProductResult, DomainError>.Failure(
                    InventoryErrors.InvalidFilterMode);
        }

        // 3. Get all inventory for the product
        List<Domain.Inventory.Inventory> allInventories = await _inventoryRepository.GetByProductIdAsync(
            query.ProductId,
            cancellationToken);

        // 4. Filter by target locations
        var inventories = allInventories
            .Where(i => targetLocationIds.Contains(i.LocationId))
            .ToList();

        _logger.LogDebug(
            "Found {Count} inventory records for product {ProductId} in target locations",
            inventories.Count,
            query.ProductId);

        // 5. Map to DTOs
        var inventoryDtos = inventories.Select(i => new InventoryDto(
            i.Id,
            i.ProductId,
            i.LocationId,
            i.Quantity,
            i.ReservedQuantity,
            i.AvailableQuantity,
            i.CreatedAt,
            i.UpdatedAt)).ToList();

        // 6. Calculate totals (only for accessible locations)
        int totalQuantity = inventories.Sum(i => i.Quantity);
        int totalReservedQuantity = inventories.Sum(i => i.ReservedQuantity);
        int totalAvailableQuantity = inventories.Sum(i => i.AvailableQuantity);

        GetInventoryByProductResult result = new(
            query.ProductId,
            inventoryDtos,
            totalQuantity,
            totalReservedQuantity,
            totalAvailableQuantity);

        return Result<GetInventoryByProductResult, DomainError>.Success(result);
    }
}

