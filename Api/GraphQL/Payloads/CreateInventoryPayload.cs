using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for createInventory mutation.
/// </summary>
public sealed class CreateInventoryPayload
{
    public InventoryType? Inventory { get; init; }
    public IReadOnlyList<InventoryError> Errors { get; init; } = Array.Empty<InventoryError>();

    public static CreateInventoryPayload Success(InventoryType inventory) =>
        new() { Inventory = inventory };

    public static CreateInventoryPayload Failure(params InventoryError[] errors) =>
        new() { Errors = errors };
}

