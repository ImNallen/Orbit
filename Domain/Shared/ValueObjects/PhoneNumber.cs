using System.Text.RegularExpressions;
using Domain.Abstractions;

namespace Domain.Shared.ValueObjects;

/// <summary>
/// Value object representing a phone number.
/// </summary>
public sealed partial record PhoneNumber
{
    private const int MinLength = 10;
    private const int MaxLength = 20;

    // Regex for international phone numbers (allows digits, spaces, hyphens, parentheses, and + prefix)
    private static readonly Regex PhoneRegex = GeneratePhoneRegex();

    private PhoneNumber(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new PhoneNumber value object.
    /// </summary>
    /// <param name="phoneNumber">The phone number string.</param>
    /// <returns>Result containing the PhoneNumber or an error.</returns>
    public static Result<PhoneNumber, DomainError> Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
        {
            return Result<PhoneNumber, DomainError>.Failure(PhoneNumberErrors.PhoneNumberRequired);
        }

        // Trim whitespace
        string trimmedPhone = phoneNumber.Trim();

        // Extract only digits for length validation
        string digitsOnly = Regex.Replace(trimmedPhone, @"[^\d]", string.Empty);

        if (digitsOnly.Length < MinLength)
        {
            return Result<PhoneNumber, DomainError>.Failure(PhoneNumberErrors.PhoneNumberTooShort);
        }

        if (digitsOnly.Length > MaxLength)
        {
            return Result<PhoneNumber, DomainError>.Failure(PhoneNumberErrors.PhoneNumberTooLong);
        }

        if (!PhoneRegex.IsMatch(trimmedPhone))
        {
            return Result<PhoneNumber, DomainError>.Failure(PhoneNumberErrors.InvalidPhoneNumber);
        }

        return Result<PhoneNumber, DomainError>.Success(new PhoneNumber(trimmedPhone));
    }

    [GeneratedRegex(@"^\+?[\d\s\-\(\)]+$", RegexOptions.Compiled)]
    private static partial Regex GeneratePhoneRegex();

    public override string ToString() => Value;

    public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
}

/// <summary>
/// Contains phone number-related domain errors.
/// </summary>
public static class PhoneNumberErrors
{
    public static readonly DomainError PhoneNumberRequired = new PhoneNumberError(
        "PhoneNumber.Required",
        "Phone number is required.");

    public static readonly DomainError PhoneNumberTooShort = new PhoneNumberError(
        "PhoneNumber.TooShort",
        "Phone number must contain at least 10 digits.");

    public static readonly DomainError PhoneNumberTooLong = new PhoneNumberError(
        "PhoneNumber.TooLong",
        "Phone number cannot contain more than 20 digits.");

    public static readonly DomainError InvalidPhoneNumber = new PhoneNumberError(
        "PhoneNumber.Invalid",
        "Phone number format is invalid.");

    // Private error record
    private sealed record PhoneNumberError(string Code, string Message) : DomainError(Code, Message);
}

