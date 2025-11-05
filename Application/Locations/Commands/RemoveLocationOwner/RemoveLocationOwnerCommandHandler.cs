using Application.Abstractions;
using Domain.Abstractions;
using Domain.Locations;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.RemoveLocationOwner;

/// <summary>
/// Handler for RemoveLocationOwnerCommand.
/// </summary>
public sealed class RemoveLocationOwnerCommandHandler
    : IRequestHandler<RemoveLocationOwnerCommand, Result<RemoveLocationOwnerResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<RemoveLocationOwnerCommandHandler> _logger;

    public RemoveLocationOwnerCommandHandler(
        ILocationRepository locationRepository,
        ILogger<RemoveLocationOwnerCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<RemoveLocationOwnerResult, DomainError>> Handle(
        RemoveLocationOwnerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing owner from location {LocationId}", command.LocationId);

        // 1. Get the location
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<RemoveLocationOwnerResult, DomainError>.Failure(
                LocationErrors.LocationNotFound);
        }

        // 2. Remove the owner
        location.RemoveOwner();

        // 3. Save changes
        await _locationRepository.UpdateAsync(location, cancellationToken);

        _logger.LogInformation("Successfully removed owner from location {LocationId}", command.LocationId);

        return Result<RemoveLocationOwnerResult, DomainError>.Success(
            new RemoveLocationOwnerResult("Owner removed from location successfully."));
    }
}

