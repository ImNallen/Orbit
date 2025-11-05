namespace Domain.Permission.Enums;

/// <summary>
/// Represents the scope of a permission in relation to locations.
/// Defines what data a user can access based on their role and location assignments.
/// </summary>
public enum PermissionScope
{
    /// <summary>
    /// Global scope - user can access data from all locations.
    /// Typically for HQ Admin or Corporate roles.
    /// </summary>
    Global = 0,

    /// <summary>
    /// Owned scope - user can access data from locations they own.
    /// Typically for Store Owner role.
    /// </summary>
    Owned = 1,

    /// <summary>
    /// Managed scope - user can access data from locations they manage.
    /// Typically for Store Manager role.
    /// </summary>
    Managed = 2,

    /// <summary>
    /// Assigned scope - user can access data from all locations they are assigned to.
    /// Typically for employees who work at multiple locations.
    /// </summary>
    Assigned = 3,

    /// <summary>
    /// Context scope - user can only access data from their current active location context.
    /// Typically for regular employees who must switch context between locations.
    /// </summary>
    Context = 4
}

