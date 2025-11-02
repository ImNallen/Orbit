namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL enum for sort order direction.
/// </summary>
public enum SortOrderType
{
    /// <summary>
    /// Sort in ascending order (A-Z, 0-9, oldest first).
    /// </summary>
    Ascending,

    /// <summary>
    /// Sort in descending order (Z-A, 9-0, newest first).
    /// </summary>
    Descending
}

