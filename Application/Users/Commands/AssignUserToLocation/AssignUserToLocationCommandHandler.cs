using Domain.Abstractions;
using Domain.Locations;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.AssignUserToLocation;

/// <summary>
/// Handler for AssignUserToLocationCommand.
/// </summary>
public sealed class AssignUserToLocationCommandHandler
    : IRequestHandler<AssignUserToLocationCommand, Result<AssignUserToLocationResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly ILogger<AssignUserToLocationCommandHandler> _logger;

    public AssignUserToLocationCommandHandler(
        IUserRepository userRepository,
        ILocationRepository locationRepository,
        ILogger<AssignUserToLocationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _logger = logger;
    }

    public async Task<Result<AssignUserToLocationResult, DomainError>> Handle(
        AssignUserToLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning user {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        // 1. Get user by ID
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<AssignUserToLocationResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Verify location exists
        Location? location = await _locationRepository.GetByIdAsync(command.LocationId, cancellationToken);
        if (location is null)
        {
            _logger.LogWarning("Location {LocationId} not found", command.LocationId);
            return Result<AssignUserToLocationResult, DomainError>.Failure(LocationErrors.LocationNotFound);
        }

        // 3. Assign user to location
        Result<DomainError> assignmentResult = user.AssignToLocation(
            command.LocationId,
            command.LocationRoleId,
            command.IsPrimaryLocation);

        if (assignmentResult.IsFailure)
        {
            _logger.LogWarning("Failed to assign user {UserId} to location {LocationId}: {Error}",
                command.UserId, command.LocationId, assignmentResult.Error.Message);
            return Result<AssignUserToLocationResult, DomainError>.Failure(assignmentResult.Error);
        }

        // 4. Get the newly created assignment
        UserLocationAssignment? assignment = user.LocationAssignments
            .FirstOrDefault(a => a.LocationId == command.LocationId && a.Status == Domain.Users.Enums.AssignmentStatus.Active);

        if (assignment is null)
        {
            _logger.LogError("Assignment was created but not found in user's assignments");
            return Result<AssignUserToLocationResult, DomainError>.Failure(
                UserLocationAssignmentErrors.AssignmentNotFound);
        }

        // 5. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Successfully assigned user {UserId} to location {LocationId} with assignment ID {AssignmentId}",
            command.UserId, command.LocationId, assignment.Id);

        return Result<AssignUserToLocationResult, DomainError>.Success(
            new AssignUserToLocationResult(
                assignment.Id,
                command.UserId,
                command.LocationId,
                "User successfully assigned to location."));
    }
}

