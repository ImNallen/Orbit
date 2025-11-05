using Domain.Abstractions;
using Domain.Inventory;
using Domain.Locations;
using Domain.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Commands.CreateInventory;

/// <summary>
/// Handler for CreateInventoryCommand.
/// </summary>
public sealed class CreateInventoryCommandHandler
    : IRequestHandler<CreateInventoryCommand, Result<CreateInventoryResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<CreateInventoryCommandHandler> _logger;

    public CreateInventoryCommandHandler(
        IInventoryRepository inventoryRepository,
        IProductRepository productRepository,
        ILocationRepository locationRepository,
        ILogger<CreateInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _productRepository = productRepository;
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<CreateInventoryResult, DomainError>> Handle(
        CreateInventoryCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating inventory for product {ProductId} at location {LocationId}",
            command.ProductId, command.LocationId);

        // 1. Verify product exists
        Product? product = await _productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null)
        {
            _logger.LogWarning("Product {ProductId} not found", command.ProductId);
            return Result<CreateInventoryResult, DomainError>.Failure(ProductErrors.ProductNotFound);
        }

        // 2. Verify location exists
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<CreateInventoryResult, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        // 3. Check if inventory already exists for this product-location combination
        bool inventoryExists = await _inventoryRepository.ExistsAsync(
            command.ProductId,
            command.LocationId,
            cancellationToken);

        if (inventoryExists)
        {
            _logger.LogWarning("Inventory already exists for product {ProductId} at location {LocationId}",
                command.ProductId, command.LocationId);
            return Result<CreateInventoryResult, DomainError>.Failure(
                InventoryErrors.InventoryAlreadyExists);
        }

        // 4. Create Inventory entity
        Result<Domain.Inventory.Inventory, DomainError> inventoryResult =
            Domain.Inventory.Inventory.Create(
                command.ProductId,
                command.LocationId,
                command.InitialQuantity);

        if (inventoryResult.IsFailure)
        {
            _logger.LogWarning("Failed to create inventory entity: {Error}", inventoryResult.Error.Message);
            return Result<CreateInventoryResult, DomainError>.Failure(inventoryResult.Error);
        }

        Domain.Inventory.Inventory inventory = inventoryResult.Value;

        // 5. Save inventory to database
        await _inventoryRepository.AddAsync(inventory, cancellationToken);

        _logger.LogInformation("Successfully created inventory {InventoryId} for product {ProductId} at location {LocationId}",
            inventory.Id, command.ProductId, command.LocationId);

        return Result<CreateInventoryResult, DomainError>.Success(
            new CreateInventoryResult(
                inventory.Id,
                inventory.ProductId,
                inventory.LocationId,
                inventory.Quantity));
    }
}

