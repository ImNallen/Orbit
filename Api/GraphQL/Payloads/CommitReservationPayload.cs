namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for commitReservation mutation.
/// </summary>
public sealed class CommitReservationPayload
{
    public string? Message { get; init; }
    public int NewQuantity { get; init; }
    public int NewReservedQuantity { get; init; }
    public int AvailableQuantity { get; init; }
    public IReadOnlyList<InventoryError> Errors { get; init; } = Array.Empty<InventoryError>();

    public static CommitReservationPayload Success(
        string message,
        int newQuantity,
        int newReservedQuantity,
        int availableQuantity) => new()
    {
        Message = message,
        NewQuantity = newQuantity,
        NewReservedQuantity = newReservedQuantity,
        AvailableQuantity = availableQuantity
    };

    public static CommitReservationPayload Failure(params InventoryError[] errors) => new()
    {
        Errors = errors
    };
}

