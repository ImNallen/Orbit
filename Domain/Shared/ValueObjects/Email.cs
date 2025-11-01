using System.Text.RegularExpressions;
using Domain.Abstractions;
using Domain.Users;

namespace Domain.Shared.ValueObjects;

/// <summary>
/// Value object representing an email address.
/// </summary>
public sealed partial record Email
{
    private const int MaxLength = 255;
    
    // RFC 5322 compliant email regex (simplified)
    private static readonly Regex EmailRegex = GenerateEmailRegex();

    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new Email value object.
    /// </summary>
    /// <param name="email">The email address string.</param>
    /// <returns>Result containing the Email or an error.</returns>
#pragma warning disable CA1308 // Email addresses are case-insensitive and lowercase is the standard
    public static Result<Email, DomainError> Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<Email, DomainError>.Failure(UserErrors.InvalidEmail);
        }

        // Normalize: trim and lowercase
        string normalizedEmail = email.Trim().ToLowerInvariant();

        if (normalizedEmail.Length > MaxLength)
        {
            return Result<Email, DomainError>.Failure(UserErrors.InvalidEmail);
        }

        if (!EmailRegex.IsMatch(normalizedEmail))
        {
            return Result<Email, DomainError>.Failure(UserErrors.InvalidEmail);
        }

        return Result<Email, DomainError>.Success(new Email(normalizedEmail));
    }
#pragma warning restore CA1308

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex GenerateEmailRegex();

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}

