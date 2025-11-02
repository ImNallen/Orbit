namespace Domain.Customers.Enums;

/// <summary>
/// Represents the status of a customer account.
/// </summary>
public enum CustomerStatus
{
    /// <summary>
    /// Customer is active and can transact normally.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Customer is temporarily inactive.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Customer is suspended and cannot transact.
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Customer has been soft-deleted and cannot be used.
    /// </summary>
    Deleted = 3
}

