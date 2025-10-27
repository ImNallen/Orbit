namespace Infrastructure.EmailConfiguration;

/// <summary>
/// Configuration options for email services.
/// </summary>
public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>
    /// The email address to send emails from.
    /// </summary>
    public string FromEmail { get; init; } = "noreply@orbit.local";

    /// <summary>
    /// The name to display as the sender.
    /// </summary>
    public string FromName { get; init; } = "Orbit";

    /// <summary>
    /// SMTP server configuration.
    /// </summary>
    public SmtpOptions Smtp { get; init; } = new();
}

/// <summary>
/// SMTP server configuration options.
/// </summary>
public sealed class SmtpOptions
{
    /// <summary>
    /// SMTP server host.
    /// </summary>
    public string Host { get; init; } = "localhost";

    /// <summary>
    /// SMTP server port.
    /// </summary>
    public int Port { get; init; } = 25;

    /// <summary>
    /// SMTP username (if authentication is required).
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// SMTP password (if authentication is required).
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Whether to use SSL/TLS.
    /// </summary>
    public bool EnableSsl { get; init; }
}

