namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for releaseReservation mutation.
/// </summary>
public sealed class ReleaseReservationPayload
{
    public string? Message { get; init; }
    public int ReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public IReadOnlyList<InventoryError> Errors { get; init; } = Array.Empty<InventoryError>();

    public static ReleaseReservationPayload Success(string message, int reservedQuantity, int availableQuantity) =>
        new() { Message = message, ReservedQuantity = reservedQuantity, AvailableQuantity = availableQuantity };

    public static ReleaseReservationPayload Failure(params InventoryError[] errors) =>
        new() { Errors = errors };
}

