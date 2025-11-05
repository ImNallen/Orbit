using Api.GraphQL.Inputs;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Inventory.Commands.CreateInventory;
using Application.Inventory.Commands.AdjustStock;
using Application.Inventory.Commands.ReserveStock;
using Application.Inventory.Commands.ReleaseReservation;
using Application.Inventory.Commands.TransferStock;
using Application.Inventory.Commands.CommitReservation;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for inventory operations.
/// </summary>
[ExtendObjectType("Mutation")]
public sealed class InventoryMutations
{
    /// <summary>
    /// Creates a new inventory record for a product at a location.
    /// Requires inventory:create permission.
    /// </summary>
    [Authorize(Policy = "inventory:create")]
    public async Task<CreateInventoryPayload> CreateInventoryAsync(
        CreateInventoryInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CreateInventoryCommand(
            input.ProductId,
            input.LocationId,
            input.InitialQuantity);

        Result<CreateInventoryResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return CreateInventoryPayload.Failure(
                new InventoryError(result.Error.Code, result.Error.Message));
        }

        CreateInventoryResult inventoryResult = result.Value;

        var inventory = new InventoryType
        {
            InventoryId = inventoryResult.InventoryId,
            ProductId = input.ProductId,
            LocationId = input.LocationId,
            Quantity = input.InitialQuantity,
            ReservedQuantity = 0,
            AvailableQuantity = input.InitialQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return CreateInventoryPayload.Success(inventory);
    }

    /// <summary>
    /// Adjusts stock quantity (increase or decrease).
    /// Requires inventory:update permission.
    /// </summary>
    [Authorize(Policy = "inventory:update")]
    public async Task<AdjustStockPayload> AdjustStockAsync(
        AdjustStockInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AdjustStockCommand(
            input.InventoryId,
            input.Adjustment,
            input.Reason);

        Result<AdjustStockResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return AdjustStockPayload.Failure(
                new InventoryError(result.Error.Code, result.Error.Message));
        }

        AdjustStockResult adjustResult = result.Value;

        return AdjustStockPayload.Success(
            $"Stock adjusted successfully. New quantity: {adjustResult.NewQuantity}",
            adjustResult.NewQuantity,
            adjustResult.AvailableQuantity);
    }

    /// <summary>
    /// Reserves stock for an order or allocation.
    /// Requires inventory:update permission.
    /// </summary>
    [Authorize(Policy = "inventory:update")]
    public async Task<ReserveStockPayload> ReserveStockAsync(
        ReserveStockInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReserveStockCommand(
            input.InventoryId,
            input.Quantity);

        Result<ReserveStockResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ReserveStockPayload.Failure(
                new InventoryError(result.Error.Code, result.Error.Message));
        }

        ReserveStockResult reserveResult = result.Value;

        return ReserveStockPayload.Success(
            $"Stock reserved successfully. Reserved: {reserveResult.ReservedQuantity}",
            reserveResult.ReservedQuantity,
            reserveResult.AvailableQuantity);
    }

    /// <summary>
    /// Releases a stock reservation (e.g., cancelled order).
    /// Requires inventory:update permission.
    /// </summary>
    [Authorize(Policy = "inventory:update")]
    public async Task<ReleaseReservationPayload> ReleaseReservationAsync(
        ReleaseReservationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ReleaseReservationCommand(
            input.InventoryId,
            input.Quantity);

        Result<ReleaseReservationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ReleaseReservationPayload.Failure(
                new InventoryError(result.Error.Code, result.Error.Message));
        }

        ReleaseReservationResult releaseResult = result.Value;

        return ReleaseReservationPayload.Success(
            $"Reservation released successfully. Reserved: {releaseResult.ReservedQuantity}",
            releaseResult.ReservedQuantity,
            releaseResult.AvailableQuantity);
    }

    /// <summary>
    /// Transfers stock from one location to another.
    /// Requires inventory:update permission.
    /// </summary>
    [Authorize(Policy = "inventory:update")]
    public async Task<TransferStockPayload> TransferStockAsync(
        TransferStockInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new TransferStockCommand(
            input.FromInventoryId,
            input.ToInventoryId,
            input.Quantity,
            input.Reason ?? "Stock transfer");

        Result<TransferStockResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return TransferStockPayload.Failure(
                new InventoryError(result.Error.Code, result.Error.Message));
        }

        TransferStockResult transferResult = result.Value;

        return TransferStockPayload.Success(
            transferResult.FromInventoryId,
            transferResult.ToInventoryId,
            transferResult.Quantity,
            transferResult.FromNewQuantity,
            transferResult.ToNewQuantity);
    }

    /// <summary>
    /// Commits a stock reservation (e.g., when an order is fulfilled).
    /// This reduces both the reserved quantity and the total quantity.
    /// Requires inventory:update permission.
    /// </summary>
    [Authorize(Policy = "inventory:update")]
    public async Task<CommitReservationPayload> CommitReservationAsync(
        CommitReservationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new CommitReservationCommand(
            input.InventoryId,
            input.Quantity);

        Result<CommitReservationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return CommitReservationPayload.Failure(
                new InventoryError(result.Error.Code, result.Error.Message));
        }

        CommitReservationResult commitResult = result.Value;

        return CommitReservationPayload.Success(
            $"Reservation committed successfully. New quantity: {commitResult.NewQuantity}",
            commitResult.NewQuantity,
            commitResult.NewReservedQuantity,
            commitResult.AvailableQuantity);
    }
}

