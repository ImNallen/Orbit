using Domain.Abstractions;
using Domain.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Commands.CommitReservation;

/// <summary>
/// Handler for CommitReservationCommand.
/// Commits a reservation by reducing both reserved and total quantity.
/// This is typically called when an order is fulfilled.
/// </summary>
public sealed class CommitReservationCommandHandler
    : IRequestHandler<CommitReservationCommand, Result<CommitReservationResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<CommitReservationCommandHandler> _logger;

    public CommitReservationCommandHandler(
        IInventoryRepository inventoryRepository,
        ILogger<CommitReservationCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Result<CommitReservationResult, DomainError>> Handle(
        CommitReservationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Committing {Quantity} units reservation for inventory {InventoryId}",
            command.Quantity,
            command.InventoryId);

        // 1. Get inventory
        Domain.Inventory.Inventory? inventory = await _inventoryRepository.GetByIdAsync(
            command.InventoryId,
            cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning("Inventory {InventoryId} not found", command.InventoryId);
            return Result<CommitReservationResult, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 2. Commit the reservation
        Result<DomainError> commitResult = inventory.CommitReservation(command.Quantity);
        if (commitResult.IsFailure)
        {
            _logger.LogWarning("Failed to commit reservation: {Error}", commitResult.Error.Message);
            return Result<CommitReservationResult, DomainError>.Failure(commitResult.Error);
        }

        // 3. Save changes
        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

        _logger.LogInformation(
            "Successfully committed {Quantity} units reservation for inventory {InventoryId}. New quantity: {NewQuantity}",
            command.Quantity,
            inventory.Id,
            inventory.Quantity);

        return Result<CommitReservationResult, DomainError>.Success(
            new CommitReservationResult(
                inventory.Id,
                inventory.Quantity,
                inventory.ReservedQuantity,
                inventory.AvailableQuantity));
    }
}

