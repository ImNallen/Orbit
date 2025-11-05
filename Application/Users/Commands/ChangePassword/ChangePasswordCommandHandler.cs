using Domain.Abstractions;
using Domain.Users;
using Domain.Users.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.ChangePassword;

/// <summary>
/// Handler for ChangePasswordCommand.
/// </summary>
public sealed class ChangePasswordCommandHandler
    : IRequestHandler<ChangePasswordCommand, Result<ChangePasswordResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ChangePasswordCommandHandler> _logger;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<ChangePasswordResult, DomainError>> Handle(
        ChangePasswordCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password change attempt for user {UserId}", command.UserId);

        // 1. Get user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Password change failed: User {UserId} not found", command.UserId);
            return Result<ChangePasswordResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Validate current password
        Result<Password, DomainError> currentPasswordResult = Password.Create(command.CurrentPassword);
        if (currentPasswordResult.IsFailure)
        {
            _logger.LogWarning("Password change failed for user {UserId}: Invalid current password format",
                user.Id);
            return Result<ChangePasswordResult, DomainError>.Failure(UserErrors.InvalidCredentials);
        }

        bool isCurrentPasswordValid = _passwordHasher.Verify(currentPasswordResult.Value, user.PasswordHash);
        if (!isCurrentPasswordValid)
        {
            _logger.LogWarning("Password change failed for user {UserId}: Current password is incorrect",
                user.Id);
            return Result<ChangePasswordResult, DomainError>.Failure(UserErrors.InvalidCredentials);
        }

        // 3. Validate new password
        Result<Password, DomainError> newPasswordResult = Password.Create(command.NewPassword);
        if (newPasswordResult.IsFailure)
        {
            _logger.LogWarning("Password change failed for user {UserId}: Invalid new password format - {ErrorCode}",
                user.Id, newPasswordResult.Error.Code);
            return Result<ChangePasswordResult, DomainError>.Failure(newPasswordResult.Error);
        }

        // 4. Hash the new password
        PasswordHash newPasswordHash = _passwordHasher.Hash(newPasswordResult.Value);

        // 5. Change the password (domain logic handles password history)
        Result<DomainError> changePasswordResult = user.ChangePassword(newPasswordHash);
        if (changePasswordResult.IsFailure)
        {
            _logger.LogWarning("Password change failed for user {UserId}: {ErrorCode} - {ErrorMessage}",
                user.Id, changePasswordResult.Error.Code, changePasswordResult.Error.Message);
            return Result<ChangePasswordResult, DomainError>.Failure(changePasswordResult.Error);
        }

        // 6. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Password changed successfully for user {UserId}", user.Id);

        return Result<ChangePasswordResult, DomainError>.Success(
            new ChangePasswordResult("Password changed successfully."));
    }
}

