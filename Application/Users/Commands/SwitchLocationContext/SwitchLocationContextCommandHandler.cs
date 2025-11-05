using Domain.Abstractions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.SwitchLocationContext;

/// <summary>
/// Handler for SwitchLocationContextCommand.
/// </summary>
public sealed class SwitchLocationContextCommandHandler
    : IRequestHandler<SwitchLocationContextCommand, Result<SwitchLocationContextResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SwitchLocationContextCommandHandler> _logger;

    public SwitchLocationContextCommandHandler(
        IUserRepository userRepository,
        ILogger<SwitchLocationContextCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<SwitchLocationContextResult, DomainError>> Handle(
        SwitchLocationContextCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Switching location context for user {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        // 1. Get user by ID
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<SwitchLocationContextResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Switch location context
        Result<DomainError> switchResult = user.SwitchLocationContext(command.LocationId);
        if (switchResult.IsFailure)
        {
            _logger.LogWarning("Failed to switch location context for user {UserId} to location {LocationId}: {Error}",
                command.UserId, command.LocationId, switchResult.Error.Message);
            return Result<SwitchLocationContextResult, DomainError>.Failure(switchResult.Error);
        }

        // 3. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Successfully switched location context for user {UserId} to location {LocationId}",
            command.UserId, command.LocationId);

        return Result<SwitchLocationContextResult, DomainError>.Success(
            new SwitchLocationContextResult(
                command.UserId,
                command.LocationId,
                "Location context switched successfully."));
    }
}

