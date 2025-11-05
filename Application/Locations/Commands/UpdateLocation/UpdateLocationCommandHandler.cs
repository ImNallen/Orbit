using Domain.Abstractions;
using Domain.Locations;
using Domain.Locations.ValueObjects;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.UpdateLocation;

/// <summary>
/// Handler for UpdateLocationCommand.
/// </summary>
public sealed class UpdateLocationCommandHandler
    : IRequestHandler<UpdateLocationCommand, Result<UpdateLocationResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<UpdateLocationCommandHandler> _logger;

    public UpdateLocationCommandHandler(
        ILocationRepository locationRepository,
        ILogger<UpdateLocationCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateLocationResult, DomainError>> Handle(
        UpdateLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating location {LocationId}", command.LocationId);

        // 1. Get location
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<UpdateLocationResult, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        // 2. Create value objects
        Result<LocationName, DomainError> nameResult = LocationName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            _logger.LogWarning("Invalid location name: {Error}", nameResult.Error.Message);
            return Result<UpdateLocationResult, DomainError>.Failure(nameResult.Error);
        }

        Result<Address, DomainError> addressResult = Address.Create(
            command.Street,
            command.City,
            command.State,
            command.Country,
            command.ZipCode);

        if (addressResult.IsFailure)
        {
            _logger.LogWarning("Invalid address: {Error}", addressResult.Error.Message);
            return Result<UpdateLocationResult, DomainError>.Failure(addressResult.Error);
        }

        // 3. Update location
        Result<DomainError> updateResult = location.UpdateInfo(
            nameResult.Value,
            addressResult.Value);

        if (updateResult.IsFailure)
        {
            _logger.LogWarning("Failed to update location: {Error}", updateResult.Error.Message);
            return Result<UpdateLocationResult, DomainError>.Failure(updateResult.Error);
        }

        // 4. Save changes
        await _locationRepository.UpdateAsync(location, cancellationToken);

        _logger.LogInformation("Successfully updated location {LocationId}", location.Id);

        return Result<UpdateLocationResult, DomainError>.Success(
            new UpdateLocationResult(
                location.Id,
                location.Name.Value));
    }
}

