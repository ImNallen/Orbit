using Domain.Abstractions;

namespace Domain.UserLocations;

/// <summary>
/// Contains domain errors related to user-location assignments.
/// </summary>
public static class UserLocationAssignmentErrors
{
    public static readonly DomainError AssignmentAlreadyActive = new UserLocationAssignmentError(
        "UserLocationAssignment.AlreadyActive",
        "The assignment is already active.");

    public static readonly DomainError AssignmentAlreadyInactive = new UserLocationAssignmentError(
        "UserLocationAssignment.AlreadyInactive",
        "The assignment is already inactive.");

    public static readonly DomainError AssignmentAlreadyTerminated = new UserLocationAssignmentError(
        "UserLocationAssignment.AlreadyTerminated",
        "The assignment has already been terminated.");

    public static readonly DomainError CannotDeactivateTerminatedAssignment = new UserLocationAssignmentError(
        "UserLocationAssignment.CannotDeactivateTerminated",
        "Cannot deactivate a terminated assignment.");

    public static readonly DomainError UserNotAssignedToLocation = new UserLocationAssignmentError(
        "UserLocationAssignment.NotAssigned",
        "User is not assigned to this location.");

    public static readonly DomainError UserAlreadyAssignedToLocation = new UserLocationAssignmentError(
        "UserLocationAssignment.AlreadyAssigned",
        "User is already assigned to this location.");

    public static readonly DomainError CannotSwitchToUnassignedLocation = new UserLocationAssignmentError(
        "UserLocationAssignment.CannotSwitchToUnassigned",
        "Cannot switch context to a location the user is not assigned to.");

    public static readonly DomainError CannotSwitchToInactiveAssignment = new UserLocationAssignmentError(
        "UserLocationAssignment.CannotSwitchToInactive",
        "Cannot switch context to an inactive location assignment.");

    public static readonly DomainError MultiplePrimaryLocationsNotAllowed = new UserLocationAssignmentError(
        "UserLocationAssignment.MultiplePrimaryLocations",
        "A user can only have one primary location.");

    public static readonly DomainError CannotRemoveLastActiveAssignment = new UserLocationAssignmentError(
        "UserLocationAssignment.CannotRemoveLastActive",
        "Cannot remove the user's last active location assignment.");

    public static readonly DomainError AssignmentNotFound = new UserLocationAssignmentError(
        "UserLocationAssignment.NotFound",
        "The assignment was not found.");

    // Private error record
    private sealed record UserLocationAssignmentError(string Code, string Message) : DomainError(Code, Message);
}

