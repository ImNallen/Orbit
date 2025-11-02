using Domain.Abstractions;

namespace Domain.Customers;

/// <summary>
/// Contains all customer-related domain errors.
/// </summary>
public static class CustomerErrors
{
    // Customer Management Errors
    public static readonly DomainError CustomerNotFound = new CustomerError(
        "Customer.NotFound",
        "Customer not found.");

    public static readonly DomainError EmailAlreadyExists = new CustomerError(
        "Customer.EmailAlreadyExists",
        "A customer with this email address already exists.");

    public static readonly DomainError InvalidCustomerData = new CustomerError(
        "Customer.InvalidData",
        "The provided customer data is invalid.");

    // Status Management Errors
    public static readonly DomainError CustomerAlreadySuspended = new CustomerError(
        "Customer.AlreadySuspended",
        "Customer is already suspended.");

    public static readonly DomainError CustomerAlreadyActive = new CustomerError(
        "Customer.AlreadyActive",
        "Customer is already active.");

    public static readonly DomainError CustomerAlreadyDeleted = new CustomerError(
        "Customer.AlreadyDeleted",
        "Customer has already been deleted.");

    public static readonly DomainError CustomerDeleted = new CustomerError(
        "Customer.Deleted",
        "Customer has been deleted and cannot perform this action.");

    // Private error record
    private sealed record CustomerError(string Code, string Message) : DomainError(Code, Message);
}

