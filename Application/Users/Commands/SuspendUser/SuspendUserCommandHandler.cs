using Domain.Abstractions;
using Domain.Users;
using Domain.Users.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.SuspendUser;

/// <summary>
/// Handler for SuspendUserCommand.
/// </summary>
public sealed class SuspendUserCommandHandler
    : IRequestHandler<SuspendUserCommand, Result<SuspendUserResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<SuspendUserCommandHandler> _logger;

    public SuspendUserCommandHandler(
        IUserRepository _userRepository,
        ILogger<SuspendUserCommandHandler> logger)
    {
        this._userRepository = _userRepository;
        _logger = logger;
    }

    public async Task<Result<SuspendUserResult, DomainError>> Handle(
        SuspendUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Suspending user {UserId}", command.UserId);

        // 1. Get user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<SuspendUserResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Check if already suspended
        if (user.Status == UserStatus.Suspended)
        {
            _logger.LogWarning("User {UserId} is already suspended", command.UserId);
            return Result<SuspendUserResult, DomainError>.Failure(UserErrors.UserAlreadySuspended);
        }

        // 3. Check if deleted
        if (user.Status == UserStatus.Deleted)
        {
            _logger.LogWarning("Cannot suspend deleted user {UserId}", command.UserId);
            return Result<SuspendUserResult, DomainError>.Failure(UserErrors.AccountDeleted);
        }

        // 4. Suspend user
        user.Suspend();

        // 5. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} suspended successfully", command.UserId);

        return Result<SuspendUserResult, DomainError>.Success(
            new SuspendUserResult($"User {user.Email.Value} has been suspended"));
    }
}

