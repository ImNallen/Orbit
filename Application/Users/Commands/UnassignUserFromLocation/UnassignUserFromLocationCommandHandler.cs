using Domain.Abstractions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UnassignUserFromLocation;

/// <summary>
/// Handler for UnassignUserFromLocationCommand.
/// </summary>
public sealed class UnassignUserFromLocationCommandHandler
    : IRequestHandler<UnassignUserFromLocationCommand, Result<UnassignUserFromLocationResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UnassignUserFromLocationCommandHandler> _logger;

    public UnassignUserFromLocationCommandHandler(
        IUserRepository userRepository,
        ILogger<UnassignUserFromLocationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UnassignUserFromLocationResult, DomainError>> Handle(
        UnassignUserFromLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unassigning user {UserId} from location {LocationId}",
            command.UserId, command.LocationId);

        // 1. Get user by ID
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<UnassignUserFromLocationResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Unassign user from location
        Result<DomainError> unassignResult = user.UnassignFromLocation(command.LocationId);
        if (unassignResult.IsFailure)
        {
            _logger.LogWarning("Failed to unassign user {UserId} from location {LocationId}: {Error}",
                command.UserId, command.LocationId, unassignResult.Error.Message);
            return Result<UnassignUserFromLocationResult, DomainError>.Failure(unassignResult.Error);
        }

        // 3. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Successfully unassigned user {UserId} from location {LocationId}",
            command.UserId, command.LocationId);

        return Result<UnassignUserFromLocationResult, DomainError>.Success(
            new UnassignUserFromLocationResult("User successfully unassigned from location."));
    }
}

