namespace Application.Abstractions.Email;

/// <summary>
/// Email service interface for sending emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email verification email to the user.
    /// </summary>
    Task SendEmailVerificationAsync(
        string email,
        string firstName,
        string verificationToken,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a password reset email to the user.
    /// </summary>
    Task SendPasswordResetAsync(
        string email,
        string firstName,
        string resetToken,
        CancellationToken cancellationToken = default);
}

