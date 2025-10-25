using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.ResetPassword;

/// <summary>
/// Handler for ResetPasswordCommand.
/// </summary>
public sealed class ResetPasswordCommandHandler
    : IRequestHandler<ResetPasswordCommand, Result<ResetPasswordResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<Result<ResetPasswordResult, DomainError>> Handle(
        ResetPasswordCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset attempt");

        // 1. Validate token is not empty
        if (string.IsNullOrWhiteSpace(command.Token))
        {
            _logger.LogWarning("Password reset failed: Empty token");
            return Result<ResetPasswordResult, DomainError>.Failure(UserErrors.InvalidPasswordResetToken);
        }

        // 2. Find user by password reset token
        User? user = await _userRepository.GetByPasswordResetTokenAsync(command.Token, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Password reset failed: User not found for token");
            return Result<ResetPasswordResult, DomainError>.Failure(UserErrors.InvalidPasswordResetToken);
        }

        // 3. Validate the password reset token
        Result<DomainError> tokenValidationResult = user.ValidatePasswordResetToken(command.Token);
        if (tokenValidationResult.IsFailure)
        {
            _logger.LogWarning("Password reset failed for user {UserId}: {ErrorCode} - {ErrorMessage}",
                user.Id, tokenValidationResult.Error.Code, tokenValidationResult.Error.Message);
            return Result<ResetPasswordResult, DomainError>.Failure(tokenValidationResult.Error);
        }

        // 4. Create Password value object
        Result<Password, DomainError> passwordResult = Password.Create(command.NewPassword);
        if (passwordResult.IsFailure)
        {
            _logger.LogWarning("Password reset failed for user {UserId}: Invalid password format - {ErrorCode}",
                user.Id, passwordResult.Error.Code);
            return Result<ResetPasswordResult, DomainError>.Failure(passwordResult.Error);
        }

        // 5. Hash the new password
        PasswordHash newPasswordHash = _passwordHasher.Hash(passwordResult.Value);

        // 6. Change the password
        Result<DomainError> changePasswordResult = user.ChangePassword(newPasswordHash);
        if (changePasswordResult.IsFailure)
        {
            _logger.LogWarning("Password reset failed for user {UserId}: {ErrorCode} - {ErrorMessage}",
                user.Id, changePasswordResult.Error.Code, changePasswordResult.Error.Message);
            return Result<ResetPasswordResult, DomainError>.Failure(changePasswordResult.Error);
        }

        // 7. Unlock account and reset failed login attempts (if any)
        if (user.FailedLoginAttempts > 0 || user.LockedUntil.HasValue)
        {
            user.UnlockAccount();
        }

        // 8. Save user
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Password reset successful for user {UserId} ({Email})",
            user.Id, user.Email.Value);

        return Result<ResetPasswordResult, DomainError>.Success(
            new ResetPasswordResult(
                true,
                "Password has been reset successfully.",
                []));
    }
}

