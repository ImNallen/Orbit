using Domain.Abstractions;
using Domain.Users.Errors;

namespace Domain.Users.User;

/// <summary>
/// Value object representing a person's full name.
/// </summary>
public sealed record FullName
{
    private const int MaxLength = 100;

    private FullName(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; }
    public string LastName { get; }

    /// <summary>
    /// Creates a new FullName value object.
    /// </summary>
    /// <param name="firstName">The first name.</param>
    /// <param name="lastName">The last name.</param>
    /// <returns>Result containing the FullName or an error.</returns>
    public static Result<FullName, DomainError> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<FullName, DomainError>.Failure(UserErrors.InvalidFirstName);
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<FullName, DomainError>.Failure(UserErrors.InvalidLastName);
        }

        // Trim whitespace
        string trimmedFirstName = firstName.Trim();
        string trimmedLastName = lastName.Trim();

        if (trimmedFirstName.Length > MaxLength)
        {
            return Result<FullName, DomainError>.Failure(UserErrors.NameTooLong);
        }

        if (trimmedLastName.Length > MaxLength)
        {
            return Result<FullName, DomainError>.Failure(UserErrors.NameTooLong);
        }

        return Result<FullName, DomainError>.Success(new FullName(trimmedFirstName, trimmedLastName));
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public override string ToString() => GetFullName();
}

