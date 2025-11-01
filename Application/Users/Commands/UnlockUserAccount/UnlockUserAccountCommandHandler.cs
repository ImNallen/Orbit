using Domain.Abstractions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UnlockUserAccount;

/// <summary>
/// Handler for UnlockUserAccountCommand.
/// </summary>
public sealed class UnlockUserAccountCommandHandler
    : IRequestHandler<UnlockUserAccountCommand, Result<UnlockUserAccountResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UnlockUserAccountCommandHandler> _logger;

    public UnlockUserAccountCommandHandler(
        IUserRepository userRepository,
        ILogger<UnlockUserAccountCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UnlockUserAccountResult, DomainError>> Handle(
        UnlockUserAccountCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unlocking user account {UserId}", command.UserId);

        // 1. Get user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<UnlockUserAccountResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Check if account is locked
        if (!user.IsAccountLocked())
        {
            _logger.LogWarning("User {UserId} account is not locked", command.UserId);
            return Result<UnlockUserAccountResult, DomainError>.Failure(UserErrors.AccountNotLocked);
        }

        // 3. Unlock account
        user.UnlockAccount();

        // 4. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User account {UserId} unlocked successfully", command.UserId);

        return Result<UnlockUserAccountResult, DomainError>.Success(
            new UnlockUserAccountResult($"User {user.Email.Value} account has been unlocked"));
    }
}

