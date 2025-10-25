using System.Text.RegularExpressions;
using Domain.Abstractions;
using Domain.Users.Errors;

namespace Domain.Users.User;

/// <summary>
/// Value object representing a plain-text password with validation.
/// This should only be used temporarily during password creation/change.
/// </summary>
public sealed partial record Password
{
    private const int MinLength = 8;
    private const int MaxLength = 128;

    private Password(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new Password value object with validation.
    /// </summary>
    /// <param name="password">The plain-text password.</param>
    /// <returns>Result containing the Password or an error.</returns>
    public static Result<Password, DomainError> Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return Result<Password, DomainError>.Failure(UserErrors.WeakPassword);
        }

        if (password.Length < MinLength)
        {
            return Result<Password, DomainError>.Failure(UserErrors.PasswordTooShort);
        }

        if (password.Length > MaxLength)
        {
            return Result<Password, DomainError>.Failure(UserErrors.WeakPassword);
        }

        // Must contain at least one uppercase letter
        if (!UppercaseRegex().IsMatch(password))
        {
            return Result<Password, DomainError>.Failure(UserErrors.PasswordRequiresUppercase);
        }

        // Must contain at least one lowercase letter
        if (!LowercaseRegex().IsMatch(password))
        {
            return Result<Password, DomainError>.Failure(UserErrors.PasswordRequiresLowercase);
        }

        // Must contain at least one digit
        if (!DigitRegex().IsMatch(password))
        {
            return Result<Password, DomainError>.Failure(UserErrors.PasswordRequiresDigit);
        }

        // Must contain at least one special character
        if (!SpecialCharRegex().IsMatch(password))
        {
            return Result<Password, DomainError>.Failure(UserErrors.PasswordRequiresSpecialChar);
        }

        return Result<Password, DomainError>.Success(new Password(password));
    }

    [GeneratedRegex(@"[A-Z]", RegexOptions.Compiled)]
    private static partial Regex UppercaseRegex();

    [GeneratedRegex(@"[a-z]", RegexOptions.Compiled)]
    private static partial Regex LowercaseRegex();

    [GeneratedRegex(@"\d", RegexOptions.Compiled)]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9]", RegexOptions.Compiled)]
    private static partial Regex SpecialCharRegex();

    public override string ToString() => "***REDACTED***"; // Never expose password in logs

    public static implicit operator string(Password password) => password.Value;
}

