using Application.Abstractions;
using Domain.Abstractions;
using Domain.Locations;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.AssignLocationManager;

/// <summary>
/// Handler for AssignLocationManagerCommand.
/// </summary>
public sealed class AssignLocationManagerCommandHandler
    : IRequestHandler<AssignLocationManagerCommand, Result<AssignLocationManagerResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AssignLocationManagerCommandHandler> _logger;

    public AssignLocationManagerCommandHandler(
        ILocationRepository locationRepository,
        IUserRepository userRepository,
        ILogger<AssignLocationManagerCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<AssignLocationManagerResult, DomainError>> Handle(
        AssignLocationManagerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning manager {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        // 1. Get the location
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<AssignLocationManagerResult, DomainError>.Failure(
                LocationErrors.LocationNotFound);
        }

        // 2. Verify the user exists
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<AssignLocationManagerResult, DomainError>.Failure(
                UserErrors.UserNotFound);
        }

        // 3. Assign the manager
        location.AssignManager(command.UserId);

        // 4. Save changes
        await _locationRepository.UpdateAsync(location, cancellationToken);

        _logger.LogInformation("Successfully assigned manager {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        return Result<AssignLocationManagerResult, DomainError>.Success(
            new AssignLocationManagerResult("Manager assigned to location successfully."));
    }
}

