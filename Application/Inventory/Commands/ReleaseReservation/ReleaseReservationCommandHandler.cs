using Domain.Abstractions;
using Domain.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Commands.ReleaseReservation;

/// <summary>
/// Handler for ReleaseReservationCommand.
/// </summary>
public sealed class ReleaseReservationCommandHandler
    : IRequestHandler<ReleaseReservationCommand, Result<ReleaseReservationResult, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<ReleaseReservationCommandHandler> _logger;

    public ReleaseReservationCommandHandler(
        IInventoryRepository inventoryRepository,
        ILogger<ReleaseReservationCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Result<ReleaseReservationResult, DomainError>> Handle(
        ReleaseReservationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Releasing {Quantity} units reservation for inventory {InventoryId}",
            command.Quantity, command.InventoryId);

        // 1. Get inventory
        Domain.Inventory.Inventory? inventory = await _inventoryRepository.GetByIdAsync(
            command.InventoryId,
            cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning("Inventory {InventoryId} not found", command.InventoryId);
            return Result<ReleaseReservationResult, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 2. Release reservation
        Result<DomainError> releaseResult = inventory.ReleaseReservation(command.Quantity);
        if (releaseResult.IsFailure)
        {
            _logger.LogWarning("Failed to release reservation: {Error}", releaseResult.Error.Message);
            return Result<ReleaseReservationResult, DomainError>.Failure(releaseResult.Error);
        }

        // 3. Save changes
        await _inventoryRepository.UpdateAsync(inventory, cancellationToken);

        _logger.LogInformation("Successfully released {Quantity} units reservation for inventory {InventoryId}",
            command.Quantity, inventory.Id);

        return Result<ReleaseReservationResult, DomainError>.Success(
            new ReleaseReservationResult(
                inventory.Id,
                inventory.ReservedQuantity,
                inventory.AvailableQuantity));
    }
}

