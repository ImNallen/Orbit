namespace Domain.Users.User;

/// <summary>
/// Represents the status of a user account.
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Account is active and can be used normally.
    /// </summary>
    Active = 0,

    /// <summary>
    /// Account is temporarily suspended and cannot be used.
    /// </summary>
    Suspended = 1,

    /// <summary>
    /// Account has been soft-deleted and cannot be used.
    /// </summary>
    Deleted = 2
}

