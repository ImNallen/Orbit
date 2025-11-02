using Domain.Abstractions;
using Domain.Customers.Enums;
using Domain.Customers.Events;
using Domain.Shared.ValueObjects;

namespace Domain.Customers;

/// <summary>
/// Represents a customer in the system.
/// </summary>
public sealed class Customer : Entity
{
    private Customer(
        Guid id,
        Email email,
        FullName fullName,
        Address address)
        : base(id)
    {
        Email = email;
        FullName = fullName;
        Address = address;
        Status = CustomerStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private Customer() {}

    // Profile Information
    public Email Email { get; private set; } = null!;

    public FullName FullName { get; private set; } = null!;

    public PhoneNumber? PhoneNumber { get; private set; }

    public Address Address { get; private set; } = null!;

    // Account Status
    public CustomerStatus Status { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="email">The customer's email address.</param>
    /// <param name="fullName">The customer's full name.</param>
    /// <param name="address">The customer's address.</param>
    /// <returns>Result containing the Customer or an error.</returns>
    public static Result<Customer, DomainError> Create(
        Email email,
        FullName fullName,
        Address address)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(fullName);
        ArgumentNullException.ThrowIfNull(address);

        var customer = new Customer(
            Guid.CreateVersion7(),
            email,
            fullName,
            address);

        customer.Raise(new CustomerCreatedEvent(
            customer.Id,
            customer.Email.Value,
            customer.FullName.FirstName,
            customer.FullName.LastName));

        return Result<Customer, DomainError>.Success(customer);
    }

    /// <summary>
    /// Suspends the customer account.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> Suspend()
    {
        if (Status == CustomerStatus.Deleted)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerDeleted);
        }

        if (Status == CustomerStatus.Suspended)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerAlreadySuspended);
        }

        Status = CustomerStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Activates the customer account.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> Activate()
    {
        if (Status == CustomerStatus.Deleted)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerDeleted);
        }

        if (Status == CustomerStatus.Active)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerAlreadyActive);
        }

        Status = CustomerStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Soft-deletes the customer account.
    /// </summary>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> Delete()
    {
        if (Status == CustomerStatus.Deleted)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerAlreadyDeleted);
        }

        Status = CustomerStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Updates the customer's contact information.
    /// </summary>
    /// <param name="email">The new email address.</param>
    /// <param name="fullName">The new full name.</param>
    /// <param name="phoneNumber">The new phone number (optional).</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> UpdateContactInfo(Email email, FullName fullName, PhoneNumber? phoneNumber = null)
    {
        ArgumentNullException.ThrowIfNull(email);
        ArgumentNullException.ThrowIfNull(fullName);

        if (Status == CustomerStatus.Deleted)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerDeleted);
        }

        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;

        Raise(new CustomerContactInfoUpdatedEvent(
            Id,
            email.Value,
            fullName.FirstName,
            fullName.LastName,
            phoneNumber?.Value));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Updates the customer's address.
    /// </summary>
    /// <param name="address">The new address.</param>
    /// <returns>Result indicating success or failure.</returns>
    public Result<DomainError> UpdateAddress(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (Status == CustomerStatus.Deleted)
        {
            return Result<DomainError>.Failure(CustomerErrors.CustomerDeleted);
        }

        Address = address;
        UpdatedAt = DateTime.UtcNow;

        Raise(new CustomerAddressUpdatedEvent(
            Id,
            address.Street,
            address.City,
            address.State,
            address.Country,
            address.ZipCode));

        return Result<DomainError>.Success();
    }
}
