using Application.Locations.Queries.GetLocationById;
using Domain.Abstractions;
using Domain.Locations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Queries.GetLocations;

/// <summary>
/// Handler for GetLocationsQuery.
/// </summary>
public sealed class GetLocationsQueryHandler
    : IRequestHandler<GetLocationsQuery, Result<GetLocationsResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetLocationsQueryHandler> _logger;

    public GetLocationsQueryHandler(
        ILocationRepository locationRepository,
        ILogger<GetLocationsQueryHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<GetLocationsResult, DomainError>> Handle(
        GetLocationsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting locations with filters");

        string sortBy = query.SortBy.ToString();

        (List<Location> locations, int totalCount) = await _locationRepository.QueryAsync(
            searchTerm: query.SearchTerm,
            type: query.Type,
            status: query.Status,
            city: query.City,
            state: query.State,
            country: query.Country,
            sortBy: sortBy,
            sortDescending: query.SortDescending,
            skip: query.Skip,
            take: query.Take,
            cancellationToken: cancellationToken);

        var locationDtos = locations.Select(l => new LocationDto(
            l.Id,
            l.Name.Value,
            l.Type,
            l.Status,
            l.Address.Street,
            l.Address.City,
            l.Address.State,
            l.Address.Country,
            l.Address.ZipCode,
            l.IsOperational(),
            l.CreatedAt,
            l.UpdatedAt)).ToList();

        GetLocationsResult result = new(
            locationDtos,
            totalCount,
            query.Skip,
            query.Take);

        return Result<GetLocationsResult, DomainError>.Success(result);
    }
}

