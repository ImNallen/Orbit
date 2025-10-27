using Application.Abstractions.Email;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.EmailServices;

/// <summary>
/// FluentEmail-based email service for sending emails via SMTP.
/// </summary>
public class FluentEmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;
    private readonly ILogger<FluentEmailService> _logger;

    public FluentEmailService(
        IFluentEmail fluentEmail,
        ILogger<FluentEmailService> logger)
    {
        _fluentEmail = fluentEmail;
        _logger = logger;
    }

    public async Task SendEmailVerificationAsync(
        string email,
        string firstName,
        string verificationToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SendResponse? response = await _fluentEmail
                .To(email)
                .Subject("Verify your email address - Orbit")
                .Body($"""
                    <html>
                    <body style="font-family: Arial, sans-serif; line-height: 1.6; color: #333;">
                        <div style="max-width: 600px; margin: 0 auto; padding: 20px;">
                            <h2 style="color: #4A90E2;">Email Verification</h2>
                            <p>Hello {firstName},</p>
                            <p>Thank you for registering with Orbit! Please verify your email address by using the following token:</p>
                            <div style="background-color: #f4f4f4; padding: 15px; border-radius: 5px; margin: 20px 0;">
                                <strong style="font-size: 18px; color: #4A90E2;">{verificationToken}</strong>
                            </div>
                            <p><strong>This token will expire in 24 hours.</strong></p>
                            <p>If you did not create an account, please ignore this email.</p>
                            <hr style="border: none; border-top: 1px solid #ddd; margin: 30px 0;">
                            <p style="color: #888; font-size: 12px;">
                                Best regards,<br>
                                The Orbit Team
                            </p>
                        </div>
                    </body>
                    </html>
                    """, isHtml: true)
                .SendAsync(cancellationToken);

            if (response.Successful)
            {
                _logger.LogInformation(
                    "Email verification sent successfully to {Email}",
                    email);
            }
            else
            {
                _logger.LogError(
                    "Failed to send email verification to {Email}. Errors: {Errors}",
                    email,
                    string.Join(", ", response.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occurred while sending email verification to {Email}",
                email);

            // Re-throw with additional context
            throw new InvalidOperationException($"Failed to send email verification to {email}", ex);
        }
    }

    public async Task SendPasswordResetAsync(
        string email,
        string firstName,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        try
        {
            SendResponse? response = await _fluentEmail
                .To(email)
                .Subject("Reset your password - Orbit")
                .Body($"""
                    <html>
                    <body style="font-family: Arial, sans-serif; line-height: 1.6; color: #333;">
                        <div style="max-width: 600px; margin: 0 auto; padding: 20px;">
                            <h2 style="color: #E94B3C;">Password Reset</h2>
                            <p>Hello {(string.IsNullOrEmpty(firstName) ? "there" : firstName)},</p>
                            <p>We received a request to reset your password. Please use the following token to reset your password:</p>
                            <div style="background-color: #f4f4f4; padding: 15px; border-radius: 5px; margin: 20px 0;">
                                <strong style="font-size: 18px; color: #E94B3C;">{resetToken}</strong>
                            </div>
                            <p><strong>This token will expire in 1 hour.</strong></p>
                            <p>If you did not request a password reset, please ignore this email and your password will remain unchanged.</p>
                            <hr style="border: none; border-top: 1px solid #ddd; margin: 30px 0;">
                            <p style="color: #888; font-size: 12px;">
                                Best regards,<br>
                                The Orbit Team
                            </p>
                        </div>
                    </body>
                    </html>
                    """, isHtml: true)
                .SendAsync(cancellationToken);

            if (response.Successful)
            {
                _logger.LogInformation(
                    "Password reset email sent successfully to {Email}",
                    email);
            }
            else
            {
                _logger.LogError(
                    "Failed to send password reset email to {Email}. Errors: {Errors}",
                    email,
                    string.Join(", ", response.ErrorMessages));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occurred while sending password reset email to {Email}",
                email);

            // Re-throw with additional context
            throw new InvalidOperationException($"Failed to send password reset email to {email}", ex);
        }
    }
}

