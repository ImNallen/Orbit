using Domain.Abstractions;
using Domain.Locations;
using Domain.Locations.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.ChangeLocationStatus;

/// <summary>
/// Handler for ChangeLocationStatusCommand.
/// </summary>
public sealed class ChangeLocationStatusCommandHandler
    : IRequestHandler<ChangeLocationStatusCommand, Result<ChangeLocationStatusResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<ChangeLocationStatusCommandHandler> _logger;

    public ChangeLocationStatusCommandHandler(
        ILocationRepository locationRepository,
        ILogger<ChangeLocationStatusCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<ChangeLocationStatusResult, DomainError>> Handle(
        ChangeLocationStatusCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Changing location {LocationId} status to {Status}",
            command.LocationId, command.NewStatus);

        // 1. Get location
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<ChangeLocationStatusResult, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        // 2. Change status based on the new status
        Result<DomainError> statusResult = command.NewStatus switch
        {
            LocationStatus.Active => location.Activate(),
            LocationStatus.Inactive => location.Deactivate(),
            LocationStatus.UnderMaintenance => location.SetUnderMaintenance(),
            LocationStatus.Closed => location.Close(),
            _ => Result<DomainError>.Failure(LocationErrors.InvalidLocationStatus)
        };

        if (statusResult.IsFailure)
        {
            _logger.LogWarning("Failed to change location status: {Error}", statusResult.Error.Message);
            return Result<ChangeLocationStatusResult, DomainError>.Failure(statusResult.Error);
        }

        // 3. Save changes
        await _locationRepository.UpdateAsync(location, cancellationToken);

        _logger.LogInformation("Successfully changed location {LocationId} status to {Status}",
            location.Id, location.Status);

        return Result<ChangeLocationStatusResult, DomainError>.Success(
            new ChangeLocationStatusResult(
                location.Id,
                location.Status));
    }
}

