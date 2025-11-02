namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL enum for customer sort fields.
/// </summary>
public enum CustomerSortFieldType
{
    /// <summary>
    /// Sort by email address.
    /// </summary>
    Email,

    /// <summary>
    /// Sort by first name.
    /// </summary>
    FirstName,

    /// <summary>
    /// Sort by last name.
    /// </summary>
    LastName,

    /// <summary>
    /// Sort by creation date.
    /// </summary>
    CreatedAt,

    /// <summary>
    /// Sort by last update date.
    /// </summary>
    UpdatedAt
}

