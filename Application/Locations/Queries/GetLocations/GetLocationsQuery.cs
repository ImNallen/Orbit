using Application.Locations.Queries.GetLocationById;
using Domain.Abstractions;
using Domain.Locations.Enums;
using MediatR;

namespace Application.Locations.Queries.GetLocations;

/// <summary>
/// Query to get locations with filtering and pagination.
/// </summary>
public sealed record GetLocationsQuery(
    string? SearchTerm = null,
    LocationType? Type = null,
    LocationStatus? Status = null,
    string? City = null,
    string? State = null,
    string? Country = null,
    LocationSortField SortBy = LocationSortField.CreatedAt,
    bool SortDescending = true,
    int Skip = 0,
    int Take = 100) : IRequest<Result<GetLocationsResult, DomainError>>;

/// <summary>
/// Result of get locations query.
/// </summary>
public sealed record GetLocationsResult(
    List<LocationDto> Locations,
    int TotalCount,
    int Skip,
    int Take);

