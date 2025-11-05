using Domain.Abstractions;
using Domain.UserLocations.Enums;
using Domain.Users.Enums;

namespace Domain.UserLocations;

/// <summary>
/// Represents a user's assignment to a specific location.
/// Enables users to work at multiple locations and switch context between them.
/// </summary>
public sealed class UserLocationAssignment : Entity
{
    private UserLocationAssignment(
        Guid id,
        Guid userId,
        Guid locationId,
        bool isPrimaryLocation)
        : base(id)
    {
        UserId = userId;
        LocationId = locationId;
        IsPrimaryLocation = isPrimaryLocation;
        Status = AssignmentStatus.Active;
        AssignedDate = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private UserLocationAssignment() { }

    /// <summary>
    /// Gets the user ID.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Gets the location ID.
    /// </summary>
    public Guid LocationId { get; private set; }

    /// <summary>
    /// Gets whether this is the user's primary location.
    /// A user can only have one primary location.
    /// </summary>
    public bool IsPrimaryLocation { get; private set; }

    /// <summary>
    /// Gets the assignment status.
    /// </summary>
    public AssignmentStatus Status { get; private set; }

    /// <summary>
    /// Gets the date when the user was assigned to this location.
    /// </summary>
    public DateTime AssignedDate { get; private set; }

    /// <summary>
    /// Gets the date when the assignment was terminated (if applicable).
    /// </summary>
    public DateTime? TerminatedDate { get; private set; }

    /// <summary>
    /// Gets the date when the assignment was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets the date when the assignment was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Creates a new user-location assignment.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="isPrimaryLocation">Whether this is the user's primary location.</param>
    /// <returns>Result containing the assignment or an error.</returns>
    public static Result<UserLocationAssignment, DomainError> Create(
        Guid userId,
        Guid locationId,
        bool isPrimaryLocation = false)
    {
        var assignment = new UserLocationAssignment(
            Guid.CreateVersion7(),
            userId,
            locationId,
            isPrimaryLocation);

        return Result<UserLocationAssignment, DomainError>.Success(assignment);
    }

    /// <summary>
    /// Sets this assignment as the primary location.
    /// </summary>
    public void SetAsPrimary()
    {
        if (Status != AssignmentStatus.Active)
        {
            return;
        }

        IsPrimaryLocation = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Removes primary location designation.
    /// </summary>
    public void RemoveAsPrimary()
    {
        IsPrimaryLocation = false;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the assignment.
    /// </summary>
    public Result<DomainError> Activate()
    {
        if (Status == AssignmentStatus.Active)
        {
            return Result<DomainError>.Failure(
                UserLocationAssignmentErrors.AssignmentAlreadyActive);
        }

        Status = AssignmentStatus.Active;
        TerminatedDate = null;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Deactivates the assignment temporarily.
    /// </summary>
    public Result<DomainError> Deactivate()
    {
        if (Status == AssignmentStatus.Inactive)
        {
            return Result<DomainError>.Failure(
                UserLocationAssignmentErrors.AssignmentAlreadyInactive);
        }

        if (Status == AssignmentStatus.Terminated)
        {
            return Result<DomainError>.Failure(
                UserLocationAssignmentErrors.CannotDeactivateTerminatedAssignment);
        }

        Status = AssignmentStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Terminates the assignment permanently.
    /// </summary>
    public Result<DomainError> Terminate()
    {
        if (Status == AssignmentStatus.Terminated)
        {
            return Result<DomainError>.Failure(
                UserLocationAssignmentErrors.AssignmentAlreadyTerminated);
        }

        Status = AssignmentStatus.Terminated;
        TerminatedDate = DateTime.UtcNow;
        IsPrimaryLocation = false;
        UpdatedAt = DateTime.UtcNow;

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Checks if the assignment is currently active.
    /// </summary>
    public bool IsActive() => Status == AssignmentStatus.Active;
}

