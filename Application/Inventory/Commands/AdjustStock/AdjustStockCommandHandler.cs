using Domain.Abstractions;
using Domain.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Commands.AdjustStock;

/// <summary>
/// Handler for AdjustStockCommand.
/// </summary>
public sealed class AdjustStockCommandHandler
    : IRequestHandler<AdjustStockCommand, Result<AdjustStockResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<AdjustStockCommandHandler> _logger;

    public AdjustStockCommandHandler(
        IInventoryRepository inventoryRepository,
        ILogger<AdjustStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Result<AdjustStockResult, DomainError>> Handle(
        AdjustStockCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Adjusting stock for inventory {InventoryId} by {Adjustment}",
            command.InventoryId, command.Adjustment);

        // 1. Get inventory
        Domain.Inventory.Inventory? inventory = await _inventoryRepository.GetByIdAsync(
            command.InventoryId,
            cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning("Inventory {InventoryId} not found", command.InventoryId);
            return Result<AdjustStockResult, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 2. Adjust stock
        Result<DomainError> adjustResult = inventory.AdjustStock(command.Adjustment, command.Reason);
        if (adjustResult.IsFailure)
        {
            _logger.LogWarning("Failed to adjust stock: {Error}", adjustResult.Error.Message);
            return Result<AdjustStockResult, DomainError>.Failure(adjustResult.Error);
        }

        // 3. Save changes
        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

        _logger.LogInformation("Successfully adjusted stock for inventory {InventoryId}. New quantity: {Quantity}",
            inventory.Id, inventory.Quantity);

        return Result<AdjustStockResult, DomainError>.Success(
            new AdjustStockResult(
                inventory.Id,
                inventory.Quantity,
                inventory.AvailableQuantity));
    }
}

