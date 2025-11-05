using Domain.Abstractions;
using Domain.Locations;
using Domain.Locations.ValueObjects;
using Domain.Shared.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.CreateLocation;

/// <summary>
/// Handler for CreateLocationCommand.
/// </summary>
public sealed class CreateLocationCommandHandler
    : IRequestHandler<CreateLocationCommand, Result<CreateLocationResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<CreateLocationCommandHandler> _logger;

    public CreateLocationCommandHandler(
        ILocationRepository locationRepository,
        ILogger<CreateLocationCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<CreateLocationResult, DomainError>> Handle(
        CreateLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new location: {Name}", command.Name);

        // 1. Create value objects
        Result<LocationName, DomainError> nameResult = LocationName.Create(command.Name);
        if (nameResult.IsFailure)
        {
            _logger.LogWarning("Invalid location name: {Error}", nameResult.Error.Message);
            return Result<CreateLocationResult, DomainError>.Failure(nameResult.Error);
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
            return Result<CreateLocationResult, DomainError>.Failure(addressResult.Error);
        }

        // 2. Check if location with name already exists
        bool locationExists = await _locationRepository.ExistsByNameAsync(nameResult.Value.Value, cancellationToken);
        if (locationExists)
        {
            _logger.LogWarning("Location with name {Name} already exists", nameResult.Value.Value);
            return Result<CreateLocationResult, DomainError>.Failure(
                LocationErrors.LocationNameAlreadyExists);
        }

        // 3. Create Location entity
        Result<Location, DomainError> locationResult = Location.Create(
            nameResult.Value,
            command.Type,
            addressResult.Value);

        if (locationResult.IsFailure)
        {
            _logger.LogWarning("Failed to create location entity: {Error}", locationResult.Error.Message);
            return Result<CreateLocationResult, DomainError>.Failure(locationResult.Error);
        }

        Location location = locationResult.Value;

        // 4. Save location to database
        await _locationRepository.AddAsync(location, cancellationToken);

        _logger.LogInformation("Successfully created location {LocationId} with name {Name}",
            location.Id, location.Name.Value);

        return Result<CreateLocationResult, DomainError>.Success(
            new CreateLocationResult(
                location.Id,
                location.Name.Value,
                location.Type,
                location.Address.City,
                location.Address.State));
    }
}

