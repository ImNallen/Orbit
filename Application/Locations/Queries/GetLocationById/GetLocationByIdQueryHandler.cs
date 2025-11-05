using Domain.Abstractions;
using Domain.Locations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Queries.GetLocationById;

/// <summary>
/// Handler for GetLocationByIdQuery.
/// </summary>
public sealed class GetLocationByIdQueryHandler
    : IRequestHandler<GetLocationByIdQuery, Result<LocationDto, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<GetLocationByIdQueryHandler> _logger;

    public GetLocationByIdQueryHandler(
        ILocationRepository locationRepository,
        ILogger<GetLocationByIdQueryHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<LocationDto, DomainError>> Handle(
        GetLocationByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting location {LocationId}", query.LocationId);

        Location? location = await _locationRepository.GetByIdAsync(query.LocationId, cancellationToken);

        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", query.LocationId);
            return Result<LocationDto, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        LocationDto dto = new(
            location.Id,
            location.Name.Value,
            location.Type,
            location.Status,
            location.Address.Street,
            location.Address.City,
            location.Address.State,
            location.Address.Country,
            location.Address.ZipCode,
            location.IsOperational(),
            location.CreatedAt,
            location.UpdatedAt);

        return Result<LocationDto, DomainError>.Success(dto);
    }
}

