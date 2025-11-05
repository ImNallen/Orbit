using Application.Inventory.Queries.GetInventoryById;
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
    private readonly ILogger<GetInventoryByProductQueryHandler> _logger;

    public GetInventoryByProductQueryHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ILogger<GetInventoryByProductQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Result<GetInventoryByProductResult, DomainError>> Handle(
        GetInventoryByProductQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting inventory for product {ProductId}", query.ProductId);

        // Verify product exists
        Product? product = await _productRepository.GetByIdAsync(query.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", query.ProductId);
            return Result<GetInventoryByProductResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        List<Domain.Inventory.Inventory> inventories = await _inventoryRepository.GetByProductIdAsync(
            query.ProductId,
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

