using Application.Abstractions;
using Domain.Abstractions;
using Domain.Locations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.RemoveLocationManager;

/// <summary>
/// Handler for RemoveLocationManagerCommand.
/// </summary>
public sealed class RemoveLocationManagerCommandHandler
    : IRequestHandler<RemoveLocationManagerCommand, Result<RemoveLocationManagerResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<RemoveLocationManagerCommandHandler> _logger;

    public RemoveLocationManagerCommandHandler(
        ILocationRepository locationRepository,
        ILogger<RemoveLocationManagerCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<RemoveLocationManagerResult, DomainError>> Handle(
        RemoveLocationManagerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing manager from location {LocationId}", command.LocationId);

        // 1. Get the location
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<RemoveLocationManagerResult, DomainError>.Failure(
                LocationErrors.LocationNotFound);
        }

        // 2. Remove the manager
        location.RemoveManager();

        // 3. Save changes
        await _locationRepository.UpdateAsync(location, cancellationToken);

        _logger.LogInformation("Successfully removed manager from location {LocationId}", command.LocationId);

        return Result<RemoveLocationManagerResult, DomainError>.Success(
            new RemoveLocationManagerResult("Manager removed from location successfully."));
    }
}

