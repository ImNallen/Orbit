namespace Api.GraphQL.Inputs;

/// <summary>
/// Input type for committing a stock reservation.
/// </summary>
public sealed record CommitReservationInput(
    Guid InventoryId,
    int Quantity);

