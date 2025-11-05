using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Locations.Queries.GetLocationById;
using Application.Locations.Queries.GetLocations;
using Domain.Abstractions;
using Domain.Locations.Enums;
using HotChocolate.Authorization;
using MediatR;
using LocationSortField = Application.Locations.Queries.GetLocations.LocationSortField;
using DomainLocationType = Domain.Locations.Enums.LocationType;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for locations.
/// </summary>
[ExtendObjectType("Query")]
public sealed class LocationQueries
{
    /// <summary>
    /// Get all locations with pagination, search, filtering, and sorting.
    /// Requires locations:read permission.
    /// </summary>
    [Authorize(Policy = "locations:read")]
    public async Task<LocationsPayload> LocationsAsync(
        [Service] IMediator mediator,
        string? searchTerm = null,
        LocationTypeType? type = null,
        LocationStatusType? status = null,
        string? city = null,
        string? state = null,
        string? country = null,
        LocationSortFieldType sortBy = LocationSortFieldType.Name,
        SortOrderType sortOrder = SortOrderType.Ascending,
        int skip = 0,
        int take = 50,
        CancellationToken cancellationToken = default)
    {
        // Map GraphQL enums to Domain layer enums
        DomainLocationType? locationType = type.HasValue
            ? Enum.Parse<DomainLocationType>(type.Value.ToString())
            : null;

        LocationStatus? locationStatus = status.HasValue
            ? Enum.Parse<LocationStatus>(status.Value.ToString())
            : null;

        LocationSortField locationSortField = Enum.Parse<LocationSortField>(sortBy.ToString());
        bool sortDescending = sortOrder == SortOrderType.Descending;

        var query = new GetLocationsQuery(
            searchTerm,
            locationType,
            locationStatus,
            city,
            state,
            country,
            locationSortField,
            sortDescending,
            skip,
            take);

        Result<GetLocationsResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return new LocationsPayload
            {
                Locations = Array.Empty<LocationSummaryType>(),
                TotalCount = 0,
                Skip = skip,
                Take = take
            };
        }

        GetLocationsResult locationsResult = result.Value;

        var locationSummaries = locationsResult.Locations.Select(l => new LocationSummaryType
        {
            LocationId = l.LocationId,
            Name = l.Name,
            Type = l.Type.ToString(),
            Status = l.Status.ToString(),
            City = l.City,
            State = l.State
        }).ToList();

        return new LocationsPayload
        {
            Locations = locationSummaries,
            TotalCount = locationsResult.TotalCount,
            Skip = skip,
            Take = take
        };
    }

    /// <summary>
    /// Get a location by ID.
    /// Requires locations:read permission.
    /// </summary>
    [Authorize(Policy = "locations:read")]
    public async Task<Api.GraphQL.Types.LocationType?> LocationAsync(
        Guid locationId,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetLocationByIdQuery(locationId);
        Result<LocationDto, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        LocationDto location = result.Value;

        return new Api.GraphQL.Types.LocationType
        {
            LocationId = location.LocationId,
            Name = location.Name,
            Type = location.Type.ToString(),
            Status = location.Status.ToString(),
            Street = location.Street,
            City = location.City,
            State = location.State,
            Country = location.Country,
            ZipCode = location.ZipCode,
            IsOperational = location.IsOperational,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }
}

