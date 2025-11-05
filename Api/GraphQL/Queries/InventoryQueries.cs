using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Inventory.Queries.GetInventoryById;
using Application.Inventory.Queries.GetInventoryByProduct;
using Application.Inventory.Queries.GetInventoryByLocation;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for inventory.
/// </summary>
[ExtendObjectType("Query")]
public sealed class InventoryQueries
{
    /// <summary>
    /// Get an inventory record by ID.
    /// Requires inventory:read permission.
    /// </summary>
    [Authorize(Policy = "inventory:read")]
    public async Task<InventoryType?> InventoryAsync(
        Guid inventoryId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInventoryByIdQuery(inventoryId);
        Result<InventoryDto, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        InventoryDto inventory = result.Value;

        return new InventoryType
        {
            InventoryId = inventory.InventoryId,
            ProductId = inventory.ProductId,
            LocationId = inventory.LocationId,
            Quantity = inventory.Quantity,
            ReservedQuantity = inventory.ReservedQuantity,
            AvailableQuantity = inventory.AvailableQuantity,
            CreatedAt = inventory.CreatedAt,
            UpdatedAt = inventory.UpdatedAt
        };
    }

    /// <summary>
    /// Get inventory for a product.
    /// By default, returns inventory for the user's current location context.
    /// Use filterMode to view all assigned locations or specific locations.
    /// Requires inventory:read permission.
    /// </summary>
    [Authorize(Policy = "inventory:read")]
    public async Task<InventoriesPayload> InventoriesByProductAsync(
        Guid productId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken,
        Application.Common.Enums.LocationFilterMode filterMode = Application.Common.Enums.LocationFilterMode.CurrentContext,
        IEnumerable<Guid>? specificLocationIds = null)
    {
        var query = new GetInventoryByProductQuery(productId, filterMode, specificLocationIds);
        Result<GetInventoryByProductResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return new InventoriesPayload
            {
                Inventories = Array.Empty<InventoryType>(),
                TotalQuantity = 0,
                TotalReservedQuantity = 0,
                TotalAvailableQuantity = 0,
                Errors = new[] { new InventoryError(result.Error.Code, result.Error.Message) }
            };
        }

        GetInventoryByProductResult inventoryResult = result.Value;

        var inventories = inventoryResult.Inventories.Select(i => new InventoryType
        {
            InventoryId = i.InventoryId,
            ProductId = i.ProductId,
            LocationId = i.LocationId,
            Quantity = i.Quantity,
            ReservedQuantity = i.ReservedQuantity,
            AvailableQuantity = i.AvailableQuantity,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();

        return new InventoriesPayload
        {
            Inventories = inventories,
            TotalQuantity = inventoryResult.TotalQuantity,
            TotalReservedQuantity = inventoryResult.TotalReservedQuantity,
            TotalAvailableQuantity = inventoryResult.TotalAvailableQuantity
        };
    }

    /// <summary>
    /// Get all inventory at a specific location.
    /// Requires inventory:read permission.
    /// </summary>
    [Authorize(Policy = "inventory:read")]
    public async Task<IReadOnlyList<InventoryType>> InventoriesByLocationAsync(
        Guid locationId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetInventoryByLocationQuery(locationId);
        Result<GetInventoryByLocationResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return Array.Empty<InventoryType>();
        }

        GetInventoryByLocationResult inventoryResult = result.Value;

        return inventoryResult.Inventories.Select(i => new InventoryType
        {
            InventoryId = i.InventoryId,
            ProductId = i.ProductId,
            LocationId = i.LocationId,
            Quantity = i.Quantity,
            ReservedQuantity = i.ReservedQuantity,
            AvailableQuantity = i.AvailableQuantity,
            CreatedAt = i.CreatedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();
    }
}

