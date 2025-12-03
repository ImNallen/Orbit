namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for customer with full details.
/// </summary>
public sealed class CustomerType
{
    public Guid CustomerId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public AddressType Address { get; init; } = null!;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

/// <summary>
/// GraphQL type for customer summary (without full address details).
/// </summary>
public sealed class CustomerSummaryType
{
    public Guid CustomerId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// GraphQL type for address.
/// </summary>
public sealed class AddressType
{
    public string Street { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string Country { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
}

