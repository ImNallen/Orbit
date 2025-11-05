namespace Application.Common.Enums;

/// <summary>
/// Defines how location filtering should be applied to queries.
/// </summary>
public enum LocationFilterMode
{
    /// <summary>
    /// Filter to only the user's current location context.
    /// This is the default mode for most operations.
    /// </summary>
    CurrentContext = 0,

    /// <summary>
    /// Include all locations the user is assigned to.
    /// Useful for viewing aggregated data across all assigned locations.
    /// </summary>
    AllAssigned = 1,

    /// <summary>
    /// Filter to specific location(s) provided in the query.
    /// The user must have access to the specified location(s).
    /// </summary>
    Specific = 2
}

