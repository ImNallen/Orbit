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
        _logger.LogInformation("Getting inventory for product {ProductId}", query.ProductId);

        // 1. Verify product exists
        Product? product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", query.ProductId);
            return Result<GetInventoryByProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Get accessible location IDs for the current user
        IEnumerable<Guid> accessibleLocationIds = await _currentUserService.GetAccessibleLocationIdsAsync(
            "inventory:read",
            cancellationToken);

        // 3. Get all inventory for the product
        List<Domain.Inventory.Inventory> allInventories = await _inventoryRepository.GetByProductIdAsync(
            query.ProductId,
            cancellationToken);

        // 4. Filter by accessible locations
        var inventories = allInventories
            .Where(i => accessibleLocationIds.Contains(i.LocationId))
            .ToList();

        _logger.LogDebug(
            "User has access to {AccessibleCount} out of {TotalCount} inventory records for product {ProductId}",
            inventories.Count, allInventories.Count, query.ProductId);

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

