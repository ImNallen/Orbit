using Domain.Abstractions;

namespace Domain.Inventory;

/// <summary>
/// Contains all inventory-related domain errors.
/// </summary>
public static class InventoryErrors
{
    // Inventory Management Errors
    public static readonly DomainError InventoryNotFound = new InventoryError(
        "Inventory.NotFound",
        "Inventory record not found.");

    public static readonly DomainError InvalidQuantity = new InventoryError(
        "Inventory.InvalidQuantity",
        "Quantity cannot be negative.");

    public static readonly DomainError InsufficientStock = new InventoryError(
        "Inventory.InsufficientStock",
        "Insufficient stock available.");

    public static readonly DomainError InsufficientAvailableStock = new InventoryError(
        "Inventory.InsufficientAvailableStock",
        "Insufficient available stock (after reservations).");

    public static readonly DomainError InvalidReservationQuantity = new InventoryError(
        "Inventory.InvalidReservationQuantity",
        "Reservation quantity must be greater than zero.");

    public static readonly DomainError CannotReleaseMoreThanReserved = new InventoryError(
        "Inventory.CannotReleaseMoreThanReserved",
        "Cannot release more stock than is currently reserved.");

    public static readonly DomainError InventoryAlreadyExists = new InventoryError(
        "Inventory.AlreadyExists",
        "Inventory record already exists for this product at this location.");

    // Private error record
    private sealed record InventoryError(string Code, string Message) : DomainError(Code, Message);
}

