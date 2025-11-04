using Domain.Abstractions;

namespace Domain.Products.ValueObjects;

/// <summary>
/// Value object representing a currency code (ISO 4217).
/// </summary>
public sealed record Currency
{
    // Common currency codes (ISO 4217)
    private static readonly HashSet<string> ValidCurrencyCodes =
    [
        "USD", // US Dollar
        "EUR", // Euro
        "GBP", // British Pound
        "JPY", // Japanese Yen
        "CHF", // Swiss Franc
        "CAD", // Canadian Dollar
        "AUD", // Australian Dollar
        "NZD", // New Zealand Dollar
        "CNY", // Chinese Yuan
        "INR", // Indian Rupee
        "BRL", // Brazilian Real
        "MXN", // Mexican Peso
        "ZAR", // South African Rand
        "SEK", // Swedish Krona
        "NOK", // Norwegian Krone
        "DKK", // Danish Krone
        "PLN", // Polish Zloty
        "RUB", // Russian Ruble
        "TRY", // Turkish Lira
        "KRW", // South Korean Won
        "SGD", // Singapore Dollar
        "HKD", // Hong Kong Dollar
        "THB", // Thai Baht
        "MYR", // Malaysian Ringgit
        "IDR", // Indonesian Rupiah
        "PHP", // Philippine Peso
        "AED", // UAE Dirham
        "SAR", // Saudi Riyal
        "ILS", // Israeli Shekel
        "CZK", // Czech Koruna
        "HUF", // Hungarian Forint
        "RON", // Romanian Leu
        "BGN", // Bulgarian Lev
        "HRK", // Croatian Kuna
        "ISK", // Icelandic Krona
        "CLP", // Chilean Peso
        "ARS", // Argentine Peso
        "COP", // Colombian Peso
        "PEN", // Peruvian Sol
        "VND", // Vietnamese Dong
        "EGP", // Egyptian Pound
        "NGN", // Nigerian Naira
        "KES", // Kenyan Shilling
        "PKR", // Pakistani Rupee
        "BDT", // Bangladeshi Taka
        "LKR"  // Sri Lankan Rupee
    ];

    public static readonly Currency SEK = new("SEK");
    public static readonly Currency USD = new("USD");
    public static readonly Currency EUR = new("EUR");
    public static readonly Currency GBP = new("GBP");
    public static readonly Currency JPY = new("JPY");

    private Currency(string code)
    {
        Code = code;
    }

    public string Code { get; }

    /// <summary>
    /// Creates a new Currency value object.
    /// </summary>
    /// <param name="code">The ISO 4217 currency code (e.g., USD, EUR).</param>
    /// <returns>Result containing the Currency or an error.</returns>
    public static Result<Currency, DomainError> Create(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return Result<Currency, DomainError>.Failure(ProductErrors.InvalidCurrency);
        }

        string normalizedCode = code.Trim().ToUpperInvariant();

        if (normalizedCode.Length != 3)
        {
            return Result<Currency, DomainError>.Failure(ProductErrors.InvalidCurrencyFormat);
        }

        if (!ValidCurrencyCodes.Contains(normalizedCode))
        {
            return Result<Currency, DomainError>.Failure(ProductErrors.UnsupportedCurrency);
        }

        return Result<Currency, DomainError>.Success(new Currency(normalizedCode));
    }

    public override string ToString() => Code;

    public static implicit operator string(Currency currency) => currency.Code;
}

