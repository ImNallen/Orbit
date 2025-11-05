using Domain.Abstractions;
using MediatR;

namespace Application.Inventory.Commands.CreateInventory;

/// <summary>
/// Command to create a new inventory record for a product at a location.
/// </summary>
public sealed record CreateInventoryCommand(
    Guid ProductId,
    Guid LocationId,
    int InitialQuantity = 0) : IRequest<Result<CreateInventoryResult, DomainError>>;

/// <summary>
/// Result of inventory creation.
/// </summary>
public sealed record CreateInventoryResult(
    Guid InventoryId,
    Guid ProductId,
    Guid LocationId,
    int Quantity);

