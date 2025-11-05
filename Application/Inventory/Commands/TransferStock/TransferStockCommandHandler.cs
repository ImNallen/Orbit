using Domain.Abstractions;
using Domain.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Commands.TransferStock;

/// <summary>
/// Handler for TransferStockCommand.
/// Transfers stock from one inventory location to another.
/// </summary>
public sealed class TransferStockCommandHandler
    : IRequestHandler<TransferStockCommand, Result<TransferStockResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<TransferStockCommandHandler> _logger;

    public TransferStockCommandHandler(
        IInventoryRepository inventoryRepository,
        ILogger<TransferStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Result<TransferStockResult, DomainError>> Handle(
        TransferStockCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Transferring {Quantity} units from inventory {FromInventoryId} to {ToInventoryId}",
            command.Quantity,
            command.FromInventoryId,
            command.ToInventoryId);

        // 1. Validate quantity
        if (command.Quantity <= 0)
        {
            _logger.LogWarning("Transfer failed: Invalid quantity {Quantity}", command.Quantity);
            return Result<TransferStockResult, DomainError>.Failure(
                InventoryErrors.InvalidQuantity);
        }

        // 2. Validate not transferring to same inventory
        if (command.FromInventoryId == command.ToInventoryId)
        {
            _logger.LogWarning("Transfer failed: Cannot transfer to the same inventory");
            return Result<TransferStockResult, DomainError>.Failure(
                new InventoryError("Inventory.SameLocation", "Cannot transfer stock to the same location"));
        }

        // 3. Get source inventory
        Domain.Inventory.Inventory? fromInventory = await _inventoryRepository.GetByIdAsync(
            command.FromInventoryId,
            cancellationToken);

        if (fromInventory is null)
        {
            _logger.LogWarning("Transfer failed: Source inventory {FromInventoryId} not found",
                command.FromInventoryId);
            return Result<TransferStockResult, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 4. Get destination inventory
        Domain.Inventory.Inventory? toInventory = await _inventoryRepository.GetByIdAsync(
            command.ToInventoryId,
            cancellationToken);

        if (toInventory is null)
        {
            _logger.LogWarning("Transfer failed: Destination inventory {ToInventoryId} not found",
                command.ToInventoryId);
            return Result<TransferStockResult, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 5. Validate both inventories are for the same product
        if (fromInventory.ProductId != toInventory.ProductId)
        {
            _logger.LogWarning(
                "Transfer failed: Product mismatch - source has {FromProductId}, destination has {ToProductId}",
                fromInventory.ProductId,
                toInventory.ProductId);
            return Result<TransferStockResult, DomainError>.Failure(
                new InventoryError("Inventory.ProductMismatch", "Cannot transfer stock between different products"));
        }

        // 6. Reduce stock from source location
        string transferReason = string.IsNullOrWhiteSpace(command.Reason)
            ? $"Transfer to location {toInventory.LocationId}"
            : command.Reason;

        Result<DomainError> reduceResult = fromInventory.AdjustStock(-command.Quantity, transferReason);
        if (reduceResult.IsFailure)
        {
            _logger.LogWarning(
                "Transfer failed: Cannot reduce stock from source inventory - {Error}",
                reduceResult.Error.Message);
            return Result<TransferStockResult, DomainError>.Failure(reduceResult.Error);
        }

        // 7. Add stock to destination location
        Result<DomainError> addResult = toInventory.AdjustStock(
            command.Quantity,
            $"Transfer from location {fromInventory.LocationId}");

        if (addResult.IsFailure)
        {
            _logger.LogWarning(
                "Transfer failed: Cannot add stock to destination inventory - {Error}",
                addResult.Error.Message);
            // Note: In a real system, you'd want to rollback the source reduction here
            // or use a transaction/unit of work pattern
            return Result<TransferStockResult, DomainError>.Failure(addResult.Error);
        }

        // 8. Save both inventories
        // Note: In a production system, you'd want to use a transaction or unit of work
        // to ensure both updates succeed or both fail
        await _inventoryRepository.UpdateAsync(fromInventory, cancellationToken);
        await _inventoryRepository.UpdateAsync(toInventory, cancellationToken);

        _logger.LogInformation(
            "Successfully transferred {Quantity} units from inventory {FromInventoryId} to {ToInventoryId}",
            command.Quantity,
            fromInventory.Id,
            toInventory.Id);

        return Result<TransferStockResult, DomainError>.Success(
            new TransferStockResult(
                fromInventory.Id,
                toInventory.Id,
                command.Quantity,
                fromInventory.Quantity,
                toInventory.Quantity));
    }

    /// <summary>
    /// Private error record for inventory-specific errors.
    /// </summary>
    private sealed record InventoryError(string Code, string Message) : DomainError(Code, Message);
}

