namespace Application.Customers.Queries.GetCustomers;

/// <summary>
/// Result of getting customers with pagination.
/// </summary>
public sealed record GetCustomersResult(
    IReadOnlyList<CustomerDto> Customers,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

/// <summary>
/// Customer data transfer object.
/// </summary>
public sealed record CustomerDto(
    Guid CustomerId,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string Status,
    DateTime CreatedAt,
    DateTime UpdatedAt);

