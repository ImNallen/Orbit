using Domain.Abstractions;
using Domain.Locations.Enums;
using Domain.Locations.Events;
using Domain.Locations.ValueObjects;
using Domain.Shared.ValueObjects;

namespace Domain.Locations;

/// <summary>
/// Represents a physical location (store, warehouse, distribution center) in the system.
/// </summary>
public sealed class Location : Entity
{
    private Location(
        Guid id,
        LocationName name,
        LocationType type,
        Address address)
        : base(id)
    {
        Name = name;
        Type = type;
        Address = address;
        Status = LocationStatus.Active;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private Location() { }

    // Location Information
    public LocationName Name { get; private set; }

    public LocationType Type { get; private set; }

    public Address Address { get; private set; }

    // Location Status
    public LocationStatus Status { get; private set; }

    // Ownership & Management
    public Guid? OwnerId { get; private set; }

    public Guid? ManagerId { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new location.
    /// </summary>
    /// <param name="name">The location name.</param>
    /// <param name="type">The location type.</param>
    /// <param name="address">The location address.</param>
    /// <returns>Result containing the Location or an error.</returns>
    public static Result<Location, DomainError> Create(
        LocationName name,
        LocationType type,
        Address address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);

        var location = new Location(
            Guid.CreateVersion7(),
            name,
            type,
            address);

        location.Raise(new LocationCreatedEvent(
            location.Id,
            location.Name.Value,
            location.Type.ToString()));

        return Result<Location, DomainError>.Success(location);
    }

    /// <summary>
    /// Updates the location information.
    /// </summary>
    public Result<DomainError> UpdateInfo(LocationName name, Address address)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(address);

        if (Status == LocationStatus.Closed)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationClosed);
        }

        Name = name;
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationUpdatedEvent(Id, Name.Value));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Activates the location.
    /// </summary>
    public Result<DomainError> Activate()
    {
        if (Status == LocationStatus.Closed)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationClosed);
        }

        if (Status == LocationStatus.Active)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationAlreadyActive);
        }

        LocationStatus oldStatus = Status;
        Status = LocationStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationStatusChangedEvent(Id, oldStatus.ToString(), Status.ToString()));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Deactivates the location.
    /// </summary>
    public Result<DomainError> Deactivate()
    {
        if (Status == LocationStatus.Closed)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationClosed);
        }

        if (Status == LocationStatus.Inactive)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationAlreadyInactive);
        }

        LocationStatus oldStatus = Status;
        Status = LocationStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationStatusChangedEvent(Id, oldStatus.ToString(), Status.ToString()));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Marks the location as under maintenance.
    /// </summary>
    public Result<DomainError> SetUnderMaintenance()
    {
        if (Status == LocationStatus.Closed)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationClosed);
        }

        LocationStatus oldStatus = Status;
        Status = LocationStatus.UnderMaintenance;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationStatusChangedEvent(Id, oldStatus.ToString(), Status.ToString()));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Permanently closes the location.
    /// </summary>
    public Result<DomainError> Close()
    {
        if (Status == LocationStatus.Closed)
        {
            return Result<DomainError>.Failure(LocationErrors.LocationAlreadyClosed);
        }

        LocationStatus oldStatus = Status;
        Status = LocationStatus.Closed;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationStatusChangedEvent(Id, oldStatus.ToString(), Status.ToString()));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Checks if the location is operational.
    /// </summary>
    public bool IsOperational() => Status == LocationStatus.Active;

    #region Ownership & Management Methods

    /// <summary>
    /// Assigns an owner to the location.
    /// </summary>
    /// <param name="userId">The user ID of the owner.</param>
    public void AssignOwner(Guid userId)
    {
        OwnerId = userId;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationOwnerAssignedEvent(Id, userId));
    }

    /// <summary>
    /// Removes the owner from the location.
    /// </summary>
    public void RemoveOwner()
    {
        if (OwnerId is null)
        {
            return;
        }

        Guid previousOwnerId = OwnerId.Value;
        OwnerId = null;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationOwnerRemovedEvent(Id, previousOwnerId));
    }

    /// <summary>
    /// Assigns a manager to the location.
    /// </summary>
    /// <param name="userId">The user ID of the manager.</param>
    public void AssignManager(Guid userId)
    {
        ManagerId = userId;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationManagerAssignedEvent(Id, userId));
    }

    /// <summary>
    /// Removes the manager from the location.
    /// </summary>
    public void RemoveManager()
    {
        if (ManagerId is null)
        {
            return;
        }

        Guid previousManagerId = ManagerId.Value;
        ManagerId = null;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LocationManagerRemovedEvent(Id, previousManagerId));
    }

    /// <summary>
    /// Checks if a user owns this location.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>True if the user owns this location, false otherwise.</returns>
    public bool IsOwnedBy(Guid userId) => OwnerId == userId;

    /// <summary>
    /// Checks if a user manages this location.
    /// </summary>
    /// <param name="userId">The user ID to check.</param>
    /// <returns>True if the user manages this location, false otherwise.</returns>
    public bool IsManagedBy(Guid userId) => ManagerId == userId;

    #endregion
}

