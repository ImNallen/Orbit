using Application.Abstractions.Email;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EmailServices;

/// <summary>
/// Console-based email service for development/testing.
/// In production, replace with a real email service (SendGrid, AWS SES, etc.).
/// </summary>
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailVerificationAsync(
        string email,
        string firstName,
        string verificationToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            """
            ========================================
            EMAIL VERIFICATION
            ========================================
            To: {Email}
            Subject: Verify your email address
            
            Hello {FirstName},
            
            Thank you for registering! Please verify your email address by using the following token:
            
            Verification Token: {Token}
            
            This token will expire in 24 hours.
            
            If you did not create an account, please ignore this email.
            
            Best regards,
            The Orbit Team
            ========================================
            """,
            email,
            firstName,
            verificationToken);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetAsync(
        string email,
        string firstName,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            """
            ========================================
            PASSWORD RESET
            ========================================
            To: {Email}
            Subject: Reset your password

            Hello {FirstName},

            We received a request to reset your password. Please use the following token to reset your password:

            Reset Token: {Token}

            This token will expire in 1 hour.

            If you did not request a password reset, please ignore this email and your password will remain unchanged.

            Best regards,
            The Orbit Team
            ========================================
            """,
            email,
            firstName,
            resetToken);

        return Task.CompletedTask;
    }
}

