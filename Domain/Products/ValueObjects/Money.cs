using Domain.Abstractions;

namespace Domain.Products.ValueObjects;

/// <summary>
/// Value object representing a monetary amount.
/// </summary>
public sealed record Money
{
    private const decimal MinValue = 0;
    private const decimal MaxValue = 999999999.99m;

    private Money(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }
    public Currency Currency { get; }

    /// <summary>
    /// Creates a new Money value object with SEK currency (default).
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <returns>Result containing the Money or an error.</returns>
    public static Result<Money, DomainError> Create(decimal amount)
    {
        return Create(amount, Currency.SEK);
    }

    /// <summary>
    /// Creates a new Money value object with specified currency.
    /// </summary>
    /// <param name="amount">The monetary amount.</param>
    /// <param name="currency">The currency value object.</param>
    /// <returns>Result containing the Money or an error.</returns>
    public static Result<Money, DomainError> Create(decimal amount, Currency currency)
    {
        ArgumentNullException.ThrowIfNull(currency);

        if (amount < MinValue)
        {
            return Result<Money, DomainError>.Failure(ProductErrors.InvalidPrice);
        }

        if (amount > MaxValue)
        {
            return Result<Money, DomainError>.Failure(ProductErrors.PriceTooHigh);
        }

        // Round to 2 decimal places for currency
        decimal roundedAmount = Math.Round(amount, 2);

        return Result<Money, DomainError>.Success(new Money(roundedAmount, currency));
    }

    public override string ToString() => $"{Amount:F2} {Currency.Code}";

    public static implicit operator decimal(Money money) => money.Amount;

    /// <summary>
    /// Adds two Money values (must be same currency).
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency.Code != right.Currency.Code)
        {
            throw new InvalidOperationException($"Cannot add money with different currencies: {left.Currency.Code} and {right.Currency.Code}");
        }

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two Money values (must be same currency).
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency.Code != right.Currency.Code)
        {
            throw new InvalidOperationException($"Cannot subtract money with different currencies: {left.Currency.Code} and {right.Currency.Code}");
        }

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies Money by a scalar value.
    /// </summary>
    public static Money operator *(Money money, decimal multiplier) => new Money(money.Amount * multiplier, money.Currency);
}

