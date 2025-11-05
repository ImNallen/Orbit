using Domain.Abstractions;
using Domain.Inventory.Events;

namespace Domain.Inventory;

/// <summary>
/// Represents the inventory of a specific product at a specific location.
/// </summary>
public sealed class Inventory : Entity
{
    private Inventory(
        Guid id,
        Guid productId,
        Guid locationId,
        int quantity)
        : base(id)
    {
        ProductId = productId;
        LocationId = locationId;
        Quantity = quantity;
        ReservedQuantity = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private Inventory() { }

    // References
    public Guid ProductId { get; private set; }

    public Guid LocationId { get; private set; }

    // Inventory Quantities
    public int Quantity { get; private set; }

    public int ReservedQuantity { get; private set; }

    /// <summary>
    /// Gets the available quantity (total quantity minus reserved quantity).
    /// </summary>
    public int AvailableQuantity => Quantity - ReservedQuantity;

    // Timestamps
    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new inventory record.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="initialQuantity">The initial quantity.</param>
    /// <returns>Result containing the Inventory or an error.</returns>
    public static Result<Inventory, DomainError> Create(
        Guid productId,
        Guid locationId,
        int initialQuantity = 0)
    {
        if (initialQuantity < 0)
        {
            return Result<Inventory, DomainError>.Failure(InventoryErrors.InvalidQuantity);
        }

        var inventory = new Inventory(
            Guid.CreateVersion7(),
            productId,
            locationId,
            initialQuantity);

        if (initialQuantity > 0)
        {
            inventory.Raise(new StockAdjustedEvent(
                inventory.Id,
                inventory.ProductId,
                inventory.LocationId,
                0,
                initialQuantity,
                initialQuantity,
                "Initial stock"));
        }

        return Result<Inventory, DomainError>.Success(inventory);
    }

    /// <summary>
    /// Adjusts the stock quantity by a specified amount.
    /// </summary>
    /// <param name="adjustment">The adjustment amount (positive to add, negative to remove).</param>
    /// <param name="reason">The reason for the adjustment.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> AdjustStock(int adjustment, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            reason = "Stock adjustment";
        }

        int newQuantity = Quantity + adjustment;

        if (newQuantity < 0)
        {
            return Result<DomainError>.Failure(InventoryErrors.InvalidQuantity);
        }

        // Cannot reduce quantity below reserved quantity
        if (newQuantity < ReservedQuantity)
        {
            return Result<DomainError>.Failure(InventoryErrors.InsufficientStock);
        }

        int oldQuantity = Quantity;
        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;

        Raise(new StockAdjustedEvent(
            Id,
            ProductId,
            LocationId,
            oldQuantity,
            Quantity,
            adjustment,
            reason));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Reserves a specified quantity of stock.
    /// </summary>
    /// <param name="quantity">The quantity to reserve.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> ReserveStock(int quantity)
    {
        if (quantity <= 0)
        {
            return Result<DomainError>.Failure(InventoryErrors.InvalidReservationQuantity);
        }

        if (AvailableQuantity < quantity)
        {
            return Result<DomainError>.Failure(InventoryErrors.InsufficientAvailableStock);
        }

        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;

        Raise(new StockReservedEvent(
            Id,
            ProductId,
            LocationId,
            quantity,
            ReservedQuantity));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Releases a stock reservation.
    /// </summary>
    /// <param name="quantity">The quantity to release.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
        {
            return Result<DomainError>.Failure(InventoryErrors.InvalidReservationQuantity);
        }

        if (quantity > ReservedQuantity)
        {
            return Result<DomainError>.Failure(InventoryErrors.CannotReleaseMoreThanReserved);
        }

        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;

        Raise(new StockReservationReleasedEvent(
            Id,
            ProductId,
            LocationId,
            quantity,
            ReservedQuantity));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Commits a reservation by reducing both reserved and total quantity.
    /// This is typically called when an order is fulfilled.
    /// </summary>
    /// <param name="quantity">The quantity to commit.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> CommitReservation(int quantity)
    {
        if (quantity <= 0)
        {
            return Result<DomainError>.Failure(InventoryErrors.InvalidReservationQuantity);
        }

        if (quantity > ReservedQuantity)
        {
            return Result<DomainError>.Failure(InventoryErrors.CannotReleaseMoreThanReserved);
        }

        int oldQuantity = Quantity;
        ReservedQuantity -= quantity;
        Quantity -= quantity;
        UpdatedAt = DateTime.UtcNow;

        Raise(new StockAdjustedEvent(
            Id,
            ProductId,
            LocationId,
            oldQuantity,
            Quantity,
            -quantity,
            "Reservation committed"));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Checks if there is sufficient available stock.
    /// </summary>
    public bool HasAvailableStock(int quantity) => AvailableQuantity >= quantity;
}

