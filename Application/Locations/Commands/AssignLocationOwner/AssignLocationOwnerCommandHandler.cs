using Application.Abstractions;
using Domain.Abstractions;
using Domain.Locations;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Locations.Commands.AssignLocationOwner;

/// <summary>
/// Handler for AssignLocationOwnerCommand.
/// </summary>
public sealed class AssignLocationOwnerCommandHandler
    : IRequestHandler<AssignLocationOwnerCommand, Result<AssignLocationOwnerResult, DomainError>>
{
    private readonly ILocationRepository _locationRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<AssignLocationOwnerCommandHandler> _logger;

    public AssignLocationOwnerCommandHandler(
        ILocationRepository locationRepository,
        IUserRepository userRepository,
        ILogger<AssignLocationOwnerCommandHandler> logger)
    {
        _locationRepository = locationRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<AssignLocationOwnerResult, DomainError>> Handle(
        AssignLocationOwnerCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning owner {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        // 1. Get the location
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<AssignLocationOwnerResult, DomainError>.Failure(
                LocationErrors.LocationNotFound);
        }

        // 2. Verify the user exists
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<AssignLocationOwnerResult, DomainError>.Failure(
                UserErrors.UserNotFound);
        }

        // 3. Assign the owner
        location.AssignOwner(command.UserId);

        // 4. Save changes
        await _locationRepository.UpdateAsync(location, cancellationToken);

        _logger.LogInformation("Successfully assigned owner {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        return Result<AssignLocationOwnerResult, DomainError>.Success(
            new AssignLocationOwnerResult($"Owner assigned to location successfully."));
    }
}

