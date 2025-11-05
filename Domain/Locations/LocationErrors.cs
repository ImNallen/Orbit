using Domain.Abstractions;

namespace Domain.Locations;

/// <summary>
/// Contains all location-related domain errors.
/// </summary>
public static class LocationErrors
{
    // Location Management Errors
    public static readonly DomainError LocationNotFound = new LocationError(
        "Location.NotFound",
        "Location not found.");

    public static readonly DomainError InvalidLocationName = new LocationError(
        "Location.InvalidName",
        "Location name cannot be empty.");

    public static readonly DomainError LocationNameTooLong = new LocationError(
        "Location.NameTooLong",
        "Location name cannot exceed 200 characters.");

    public static readonly DomainError LocationNameAlreadyExists = new LocationError(
        "Location.NameAlreadyExists",
        "A location with this name already exists.");

    // Status Management Errors
    public static readonly DomainError LocationAlreadyActive = new LocationError(
        "Location.AlreadyActive",
        "Location is already active.");

    public static readonly DomainError LocationAlreadyInactive = new LocationError(
        "Location.AlreadyInactive",
        "Location is already inactive.");

    public static readonly DomainError LocationAlreadyClosed = new LocationError(
        "Location.AlreadyClosed",
        "Location is already closed.");

    public static readonly DomainError LocationClosed = new LocationError(
        "Location.Closed",
        "Location has been closed and cannot perform this action.");

    public static readonly DomainError InvalidLocationStatus = new LocationError(
        "Location.InvalidStatus",
        "Invalid location status.");

    // Private error record
    private sealed record LocationError(string Code, string Message) : DomainError(Code, Message);
}

