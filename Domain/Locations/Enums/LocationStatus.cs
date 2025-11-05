namespace Domain.Locations.Enums;

/// <summary>
/// Represents the operational status of a location.
/// </summary>
public enum LocationStatus
{
    /// <summary>
    /// Location is active and operational.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Location is temporarily inactive.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// Location is under maintenance.
    /// </summary>
    UnderMaintenance = 2,

    /// <summary>
    /// Location has been permanently closed.
    /// </summary>
    Closed = 3
}

