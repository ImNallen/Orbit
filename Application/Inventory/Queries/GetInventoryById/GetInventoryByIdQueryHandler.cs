using Application.Services;
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
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<GetInventoryByIdQueryHandler> _logger;

    public GetInventoryByIdQueryHandler(
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUserService,
        ILogger<GetInventoryByIdQueryHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<InventoryDto, DomainError>> Handle(
        GetInventoryByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting inventory {InventoryId}", query.InventoryId);

        // 1. Get the inventory record
        Domain.Inventory.Inventory? inventory = await _inventoryRepository.GetByIdAsync(
            query.InventoryId,
            cancellationToken);

        if (inventory is null)
        {
            _logger.LogWarning("Inventory {InventoryId} not found", query.InventoryId);
            return Result<InventoryDto, DomainError>.Failure(InventoryErrors.InventoryNotFound);
        }

        // 2. Check if user has access to the inventory's location
        IEnumerable<Guid> accessibleLocationIds = await _currentUserService.GetAccessibleLocationIdsAsync(
            cancellationToken);

        if (!accessibleLocationIds.Contains(inventory.LocationId))
        {
            _logger.LogWarning(
                "User {UserId} does not have access to inventory {InventoryId} at location {LocationId}",
                _currentUserService.GetUserId(), query.InventoryId, inventory.LocationId);
            return Result<InventoryDto, DomainError>.Failure(InventoryErrors.AccessDenied);
        }

        // 3. Map to DTO
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

