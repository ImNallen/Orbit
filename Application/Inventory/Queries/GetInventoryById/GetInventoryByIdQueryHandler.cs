using Domain.Abstractions;
using Domain.Inventory;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Inventory.Queries.GetInventoryById;

/// <summary>
/// Handler for GetInventoryByIdQuery.
/// </summary>
public sealed class GetInventoryByIdQueryHandler
    : IRequestHandler<GetInventoryByIdQuery, Result<InventoryDto, DomainError>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<GetInventoryByIdQueryHandler> _logger;

    public GetInventoryByIdQueryHandler(
        IInventoryRepository inventoryRepository,
        ILogger<GetInventoryByIdQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<Result<InventoryDto, DomainError>> Handle(
        GetInventoryByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting inventory {InventoryId}", query.InventoryId);

        Domain.Inventory.Inventory? inventory = await _inventoryRepository.GetByIdAsync(
            query.InventoryId,
            cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning("Inventory {InventoryId} not found", query.InventoryId);
            return Result<InventoryDto, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        InventoryDto dto = new(
            inventory.Id,
            inventory.ProductId,
            inventory.LocationId,
            inventory.Quantity,
            inventory.ReservedQuantity,
            inventory.AvailableQuantity,
            inventory.CreatedAt,
            inventory.UpdatedAt);

        return Result<InventoryDto, DomainError>.Success(dto);
    }
}

