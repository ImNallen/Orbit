using Domain.Abstractions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.SetPrimaryLocation;

/// <summary>
/// Handler for SetPrimaryLocationCommand.
/// </summary>
public sealed class SetPrimaryLocationCommandHandler
    : IRequestHandler<SetPrimaryLocationCommand, Result<SetPrimaryLocationResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SetPrimaryLocationCommandHandler> _logger;

    public SetPrimaryLocationCommandHandler(
        IUserRepository userRepository,
        ILogger<SetPrimaryLocationCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<SetPrimaryLocationResult, DomainError>> Handle(
        SetPrimaryLocationCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Setting primary location for user {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        // 1. Get user by ID
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<SetPrimaryLocationResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Set primary location
        Result<DomainError> setPrimaryResult = user.SetPrimaryLocation(command.LocationId);
        if (setPrimaryResult.IsFailure)
        {
            _logger.LogWarning("Failed to set primary location for user {UserId} to location {LocationId}: {Error}",
                command.UserId, command.LocationId, setPrimaryResult.Error.Message);
            return Result<SetPrimaryLocationResult, DomainError>.Failure(setPrimaryResult.Error);
        }

        // 3. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Successfully set primary location for user {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        return Result<SetPrimaryLocationResult, DomainError>.Success(
            new SetPrimaryLocationResult("Primary location set successfully."));
    }
}

