namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for releasing a stock reservation.
/// </summary>
public sealed record ReleaseReservationInput(
    Guid InventoryId,
    int Quantity);

