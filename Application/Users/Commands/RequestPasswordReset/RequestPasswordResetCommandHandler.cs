using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.RequestPasswordReset;

/// <summary>
/// Handler for RequestPasswordResetCommand.
/// Email sending is handled by PasswordResetRequestedEventHandler.
/// </summary>
public sealed class RequestPasswordResetCommandHandler
    : IRequestHandler<RequestPasswordResetCommand, Result<RequestPasswordResetResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

    public RequestPasswordResetCommandHandler(
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        ILogger<RequestPasswordResetCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<Result<RequestPasswordResetResult, DomainError>> Handle(
        RequestPasswordResetCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Password reset request for email: {Email}", command.Email);

        // 1. Validate email format
        Result<Email, DomainError> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Password reset request failed: Invalid email format for {Email}", command.Email);
            // Return success even for invalid email to prevent email enumeration
            return Result<RequestPasswordResetResult, DomainError>.Success(
                new RequestPasswordResetResult(
                    true,
                    "If an account with that email exists, a password reset link has been sent.",
                    []));
        }

        // 2. Find user by email
        User? user = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Password reset request for non-existent email: {Email}", command.Email);
            // Return success even if user doesn't exist to prevent email enumeration
            return Result<RequestPasswordResetResult, DomainError>.Success(
                new RequestPasswordResetResult(
                    true,
                    "If an account with that email exists, a password reset link has been sent.",
                    []));
        }

        // 3. Check if account is active
        if (user.Status != UserStatus.Active)
        {
            _logger.LogWarning("Password reset request for inactive user {UserId}: Status {Status}", 
                user.Id, user.Status);
            // Return success to prevent account status enumeration
            return Result<RequestPasswordResetResult, DomainError>.Success(
                new RequestPasswordResetResult(
                    true,
                    "If an account with that email exists, a password reset link has been sent.",
                    []));
        }

        // 4. Generate password reset token
        string resetToken = _tokenGenerator.GenerateToken();
        var tokenExpiration = TimeSpan.FromHours(1); // Token valid for 1 hour
        user.SetPasswordResetToken(resetToken, tokenExpiration);

        // 5. Save user (this will publish PasswordResetRequestedEvent which triggers email sending)
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Password reset token generated for user {UserId} ({Email})",
            user.Id, user.Email.Value);

        return Result<RequestPasswordResetResult, DomainError>.Success(
            new RequestPasswordResetResult(
                true,
                "If an account with that email exists, a password reset link has been sent.",
                []));
    }
}

