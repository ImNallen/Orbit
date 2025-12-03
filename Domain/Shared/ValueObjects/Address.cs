using Domain.Abstractions;

namespace Domain.Shared.ValueObjects;

/// <summary>
/// Value object representing a physical address.
/// </summary>
public sealed record Address
{
    private const int MaxStreetLength = 200;
    private const int MaxCityLength = 100;
    private const int MaxStateLength = 100;
    private const int MaxCountryLength = 100;
    private const int MaxZipCodeLength = 20;

    private Address(string street, string city, string? state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public string Street { get; }
    public string City { get; }
    public string? State { get; }
    public string Country { get; }
    public string ZipCode { get; }

    /// <summary>
    /// Creates a new Address value object.
    /// </summary>
    /// <param name="street">The street address.</param>
    /// <param name="city">The city.</param>
    /// <param name="state">The state or province (optional, not commonly used in Nordic countries).</param>
    /// <param name="country">The country.</param>
    /// <param name="zipCode">The postal/zip code.</param>
    /// <returns>Result containing the Address or an error.</returns>
    public static Result<Address, DomainError> Create(
        string street,
        string city,
        string? state,
        string country,
        string zipCode)
    {
        // Validate street
        if (string.IsNullOrWhiteSpace(street))
        {
            return Result<Address, DomainError>.Failure(AddressErrors.StreetRequired);
        }

        string trimmedStreet = street.Trim();
        if (trimmedStreet.Length > MaxStreetLength)
        {
            return Result<Address, DomainError>.Failure(AddressErrors.StreetTooLong);
        }

        // Validate city
        if (string.IsNullOrWhiteSpace(city))
        {
            return Result<Address, DomainError>.Failure(AddressErrors.CityRequired);
        }

        string trimmedCity = city.Trim();
        if (trimmedCity.Length > MaxCityLength)
        {
            return Result<Address, DomainError>.Failure(AddressErrors.CityTooLong);
        }

        // Validate state (optional)
        string? trimmedState = null;
        if (!string.IsNullOrWhiteSpace(state))
        {
            trimmedState = state.Trim();
            if (trimmedState.Length > MaxStateLength)
            {
                return Result<Address, DomainError>.Failure(AddressErrors.StateTooLong);
            }
        }

        // Validate country
        if (string.IsNullOrWhiteSpace(country))
        {
            return Result<Address, DomainError>.Failure(AddressErrors.CountryRequired);
        }

        string trimmedCountry = country.Trim();
        if (trimmedCountry.Length > MaxCountryLength)
        {
            return Result<Address, DomainError>.Failure(AddressErrors.CountryTooLong);
        }

        // Validate zip code
        if (string.IsNullOrWhiteSpace(zipCode))
        {
            return Result<Address, DomainError>.Failure(AddressErrors.ZipCodeRequired);
        }

        string trimmedZipCode = zipCode.Trim();
        if (trimmedZipCode.Length > MaxZipCodeLength)
        {
            return Result<Address, DomainError>.Failure(AddressErrors.ZipCodeTooLong);
        }

        return Result<Address, DomainError>.Success(new Address(
            trimmedStreet,
            trimmedCity,
            trimmedState,
            trimmedCountry,
            trimmedZipCode));
    }

    /// <summary>
    /// Gets the formatted full address.
    /// </summary>
    public string GetFullAddress()
    {
        string statePart = !string.IsNullOrWhiteSpace(State) ? $"{State} " : string.Empty;
        return $"{Street}, {City}, {statePart}{ZipCode}, {Country}";
    }

    public override string ToString() => GetFullAddress();
}

/// <summary>
/// Contains all address-related domain errors.
/// </summary>
public static class AddressErrors
{
    public static readonly DomainError StreetRequired = new AddressError(
        "Address.StreetRequired",
        "Street address is required.");

    public static readonly DomainError StreetTooLong = new AddressError(
        "Address.StreetTooLong",
        "Street address cannot exceed 200 characters.");

    public static readonly DomainError CityRequired = new AddressError(
        "Address.CityRequired",
        "City is required.");

    public static readonly DomainError CityTooLong = new AddressError(
        "Address.CityTooLong",
        "City cannot exceed 100 characters.");

    public static readonly DomainError StateTooLong = new AddressError(
        "Address.StateTooLong",
        "State or province cannot exceed 100 characters.");

    public static readonly DomainError CountryRequired = new AddressError(
        "Address.CountryRequired",
        "Country is required.");

    public static readonly DomainError CountryTooLong = new AddressError(
        "Address.CountryTooLong",
        "Country cannot exceed 100 characters.");

    public static readonly DomainError ZipCodeRequired = new AddressError(
        "Address.ZipCodeRequired",
        "Postal/zip code is required.");

    public static readonly DomainError ZipCodeTooLong = new AddressError(
        "Address.ZipCodeTooLong",
        "Postal/zip code cannot exceed 20 characters.");

    private sealed record AddressError(string Code, string Message) : DomainError(Code, Message);
}
