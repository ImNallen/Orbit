using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.ActivateUser;

/// <summary>
/// Handler for ActivateUserCommand.
/// </summary>
public sealed class ActivateUserCommandHandler
    : IRequestHandler<ActivateUserCommand, Result<ActivateUserResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ActivateUserCommandHandler> _logger;

    public ActivateUserCommandHandler(
        IUserRepository userRepository,
        ILogger<ActivateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<ActivateUserResult, DomainError>> Handle(
        ActivateUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Activating user {UserId}", command.UserId);

        // 1. Get user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<ActivateUserResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Check if already active
        if (user.Status == UserStatus.Active)
        {
            _logger.LogWarning("User {UserId} is already active", command.UserId);
            return Result<ActivateUserResult, DomainError>.Failure(UserErrors.UserAlreadyActive);
        }

        // 3. Check if deleted
        if (user.Status == UserStatus.Deleted)
        {
            _logger.LogWarning("Cannot activate deleted user {UserId}", command.UserId);
            return Result<ActivateUserResult, DomainError>.Failure(UserErrors.AccountDeleted);
        }

        // 4. Activate user
        user.Activate();

        // 5. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} activated successfully", command.UserId);

        return Result<ActivateUserResult, DomainError>.Success(
            new ActivateUserResult($"User {user.Email.Value} has been activated"));
    }
}

