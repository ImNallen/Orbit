using Domain.Abstractions;
using Domain.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Commands.ReserveStock;

/// <summary>
/// Handler for ReserveStockCommand.
/// </summary>
public sealed class ReserveStockCommandHandler
    : IRequestHandler<ReserveStockCommand, Result<ReserveStockResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<ReserveStockCommandHandler> _logger;

    public ReserveStockCommandHandler(
        IInventoryRepository inventoryRepository,
        ILogger<ReserveStockCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Result<ReserveStockResult, DomainError>> Handle(
        ReserveStockCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reserving {Quantity} units for inventory {InventoryId}",
            command.Quantity, command.InventoryId);

        // 1. Get inventory
        Domain.Inventory.Inventory? inventory = await _inventoryRepository.GetByIdAsync(
            command.InventoryId,
            cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning("Inventory {InventoryId} not found", command.InventoryId);
            return Result<ReserveStockResult, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 2. Reserve stock
        Result<DomainError> reserveResult = inventory.ReserveStock(command.Quantity);
        if (reserveResult.IsFailure)
        {
            _logger.LogWarning("Failed to reserve stock: {Error}", reserveResult.Error.Message);
            return Result<ReserveStockResult, DomainError>.Failure(reserveResult.Error);
        }

        // 3. Save changes
        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

        _logger.LogInformation("Successfully reserved {Quantity} units for inventory {InventoryId}",
            command.Quantity, inventory.Id);

        return Result<ReserveStockResult, DomainError>.Success(
            new ReserveStockResult(
                inventory.Id,
                inventory.ReservedQuantity,
                inventory.AvailableQuantity));
    }
}

