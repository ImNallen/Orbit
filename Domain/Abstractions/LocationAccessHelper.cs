using Domain.Locations;
using Domain.Permission.Enums;
using Domain.Users;
using Domain.Users.Enums;

namespace Domain.Abstractions;

/// <summary>
/// Helper class for location-based access control logic.
/// Contains pure domain logic for determining location access.
/// </summary>
public static class LocationAccessHelper
{
    /// <summary>
    /// Gets all location IDs a user can access based on permission scope.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="allLocations">All locations in the system.</param>
    /// <param name="scope">The permission scope.</param>
    /// <returns>Collection of accessible location IDs.</returns>
    public static IEnumerable<Guid> GetAccessibleLocationIds(
        User user,
        IEnumerable<Location> allLocations,
        PermissionScope scope)
    {
        return scope switch
        {
            PermissionScope.Global => allLocations.Select(l => l.Id),

            PermissionScope.Owned => allLocations
                .Where(l => l.OwnerId == user.Id)
                .Select(l => l.Id),

            PermissionScope.Managed => allLocations
                .Where(l => l.ManagerId == user.Id)
                .Select(l => l.Id),

            PermissionScope.Assigned => user.LocationAssignments
                .Where(a => a.Status == AssignmentStatus.Active)
                .Select(a => a.LocationId),

            PermissionScope.Context => user.CurrentLocationContextId is not null
                ? [user.CurrentLocationContextId.Value]
                : [],

            _ => []
        };
    }

    /// <summary>
    /// Checks if a user can access a specific location based on permission scope.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="location">The location to check.</param>
    /// <param name="scope">The permission scope.</param>
    /// <returns>True if the user can access the location, false otherwise.</returns>
    public static bool CanAccessLocation(
        User user,
        Location location,
        PermissionScope scope)
    {
        return scope switch
        {
            PermissionScope.Global => true,

            PermissionScope.Owned => location.OwnerId == user.Id,

            PermissionScope.Managed => location.ManagerId == user.Id,

            PermissionScope.Assigned => user.LocationAssignments
                .Any(a => a.LocationId == location.Id && a.Status == AssignmentStatus.Active),

            PermissionScope.Context => user.CurrentLocationContextId == location.Id,

            _ => false
        };
    }

    /// <summary>
    /// Determines the appropriate permission scope for a user based on their role and assignments.
    /// This is a helper method - actual scope should be determined by role permissions.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="allLocations">All locations in the system.</param>
    /// <returns>Suggested permission scope.</returns>
    public static PermissionScope DetermineDefaultScope(
        User user,
        IEnumerable<Location> allLocations)
    {
        var locations = allLocations.ToList();

        // If user owns any locations, they likely have Owned scope
        if (locations.Any(l => l.OwnerId == user.Id))
        {
            return PermissionScope.Owned;
        }

        // If user manages any locations, they likely have Managed scope
        if (locations.Any(l => l.ManagerId == user.Id))
        {
            return PermissionScope.Managed;
        }

        // If user is assigned to multiple locations, they likely have Assigned scope
        int activeAssignments = user.LocationAssignments
            .Count(a => a.Status == AssignmentStatus.Active);

        if (activeAssignments > 1)
        {
            return PermissionScope.Assigned;
        }

        // If user is assigned to one location, they likely have Context scope
        if (activeAssignments == 1)
        {
            return PermissionScope.Context;
        }

        // Default to Context (most restrictive)
        return PermissionScope.Context;
    }

    /// <summary>
    /// Checks if a user has any location access.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>True if the user has at least one active location assignment.</returns>
    public static bool HasAnyLocationAccess(User user)
    {
        return user.LocationAssignments.Any(a => a.Status == AssignmentStatus.Active);
    }

    /// <summary>
    /// Gets the user's primary location ID, if any.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Primary location ID, or null if none.</returns>
    public static Guid? GetPrimaryLocationId(User user)
    {
        return user.LocationAssignments
            .FirstOrDefault(a => a.IsPrimaryLocation && a.Status == AssignmentStatus.Active)
            ?.LocationId;
    }
}

