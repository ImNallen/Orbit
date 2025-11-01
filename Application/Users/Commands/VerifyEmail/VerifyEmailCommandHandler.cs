using Domain.Abstractions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.VerifyEmail;

/// <summary>
/// Handler for VerifyEmailCommand.
/// </summary>
public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<VerifyEmailResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<VerifyEmailCommandHandler> _logger;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        ILogger<VerifyEmailCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<VerifyEmailResult, DomainError>> Handle(
        VerifyEmailCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Email verification attempt");

        // 1. Validate token is not empty
        if (string.IsNullOrWhiteSpace(command.Token))
        {
            _logger.LogWarning("Email verification failed: Empty token");
            return Result<VerifyEmailResult, DomainError>.Failure(UserErrors.InvalidEmailVerificationToken);
        }

        // 2. Find user by verification token
        User? user = await _userRepository.GetByEmailVerificationTokenAsync(command.Token, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Email verification failed: User not found for token");
            return Result<VerifyEmailResult, DomainError>.Failure(UserErrors.InvalidEmailVerificationToken);
        }

        // 3. Verify the email using domain logic
        Result<DomainError> verificationResult = user.VerifyEmail(command.Token);
        if (verificationResult.IsFailure)
        {
            _logger.LogWarning("Email verification failed for user {UserId}: {ErrorCode} - {ErrorMessage}",
                user.Id, verificationResult.Error.Code, verificationResult.Error.Message);
            return Result<VerifyEmailResult, DomainError>.Failure(verificationResult.Error);
        }

        // 4. Save the updated user
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Email verification successful for user {UserId} ({Email})",
            user.Id, user.Email.Value);

        // 5. Return success result
        return Result<VerifyEmailResult, DomainError>.Success(
            new VerifyEmailResult(
                user.Id,
                user.Email.Value,
                true));
    }
}

