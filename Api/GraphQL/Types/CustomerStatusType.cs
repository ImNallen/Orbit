namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL enum for customer status.
/// </summary>
public enum CustomerStatusType
{
    /// <summary>
    /// Customer is active.
    /// </summary>
    Active,

    /// <summary>
    /// Customer is inactive.
    /// </summary>
    Inactive,

    /// <summary>
    /// Customer is suspended.
    /// </summary>
    Suspended,

    /// <summary>
    /// Customer is deleted.
    /// </summary>
    Deleted
}

