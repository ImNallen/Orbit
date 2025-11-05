using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for locations query.
/// </summary>
public sealed class LocationsPayload
{
    public IReadOnlyList<LocationSummaryType> Locations { get; init; } = Array.Empty<LocationSummaryType>();
    public int TotalCount { get; init; }
    public int Skip { get; init; }
    public int Take { get; init; }
}

