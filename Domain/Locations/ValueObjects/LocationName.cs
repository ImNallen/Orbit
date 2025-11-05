using Domain.Abstractions;

namespace Domain.Locations.ValueObjects;

/// <summary>
/// Value object representing a location name.
/// </summary>
public sealed record LocationName
{
    private const int MaxLength = 200;

    private LocationName(string value)
    {
        Value = value;
    }

    public string Value { get; }

    /// <summary>
    /// Creates a new LocationName value object.
    /// </summary>
    /// <param name="name">The location name.</param>
    /// <returns>Result containing the LocationName or an error.</returns>
    public static Result<LocationName, DomainError> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<LocationName, DomainError>.Failure(LocationErrors.InvalidLocationName);
        }

        string trimmedName = name.Trim();

        if (trimmedName.Length > MaxLength)
        {
            return Result<LocationName, DomainError>.Failure(LocationErrors.LocationNameTooLong);
        }

        return Result<LocationName, DomainError>.Success(new LocationName(trimmedName));
    }

    public override string ToString() => Value;

    public static implicit operator string(LocationName name) => name.Value;
}

