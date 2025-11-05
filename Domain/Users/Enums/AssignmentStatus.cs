namespace Domain.Users.Enums;

/// <summary>
/// Represents the status of a user's assignment to a location.
/// </summary>
public enum AssignmentStatus
{
    /// <summary>
    /// User is actively assigned to the location.
    /// </summary>
    Active = 0,

    /// <summary>
    /// User assignment is temporarily inactive.
    /// </summary>
    Inactive = 1,

    /// <summary>
    /// User assignment has been terminated.
    /// </summary>
    Terminated = 2
}

