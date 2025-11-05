using Api.GraphQL.Inputs;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Locations.Commands.CreateLocation;
using Application.Locations.Commands.UpdateLocation;
using Application.Locations.Commands.ChangeLocationStatus;
using Application.Locations.Commands.AssignLocationOwner;
using Application.Locations.Commands.RemoveLocationOwner;
using Application.Locations.Commands.AssignLocationManager;
using Application.Locations.Commands.RemoveLocationManager;
using Application.Users.Commands.AssignUserToLocation;
using Application.Users.Commands.UnassignUserFromLocation;
using Application.Users.Commands.SwitchLocationContext;
using Application.Users.Commands.SetPrimaryLocation;
using Domain.Abstractions;
using Domain.Locations.Enums;
using HotChocolate.Authorization;
using MediatR;
using DomainLocationType = Domain.Locations.Enums.LocationType;

namespace Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for location operations.
/// </summary>
[ExtendObjectType("Mutation")]
public sealed class LocationMutations
{
    /// <summary>
    /// Creates a new location.
    /// Requires locations:create permission.
    /// </summary>
    [Authorize(Policy = "locations:create")]
    public async Task<CreateLocationPayload> CreateLocationAsync(
        CreateLocationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        DomainLocationType locationType = input.Type switch
        {
            LocationTypeType.Store => DomainLocationType.Store,
            LocationTypeType.Warehouse => DomainLocationType.Warehouse,
            LocationTypeType.DistributionCenter => DomainLocationType.DistributionCenter,
            _ => DomainLocationType.Store
        };

        var command = new CreateLocationCommand(
            input.Name,
            locationType,
            input.Street,
            input.City,
            input.State,
            input.Country,
            input.ZipCode);

        Result<CreateLocationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return CreateLocationPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        CreateLocationResult locationResult = result.Value;

        var location = new Api.GraphQL.Types.LocationType
        {
            LocationId = locationResult.LocationId,
            Name = locationResult.Name,
            Type = locationType.ToString(),
            Status = "Active",
            Street = input.Street,
            City = input.City,
            State = input.State,
            Country = input.Country,
            ZipCode = input.ZipCode,
            IsOperational = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return CreateLocationPayload.Success(location);
    }

    /// <summary>
    /// Updates location information.
    /// Requires locations:update permission.
    /// </summary>
    [Authorize(Policy = "locations:update")]
    public async Task<UpdateLocationPayload> UpdateLocationAsync(
        UpdateLocationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLocationCommand(
            input.LocationId,
            input.Name,
            input.Street,
            input.City,
            input.State,
            input.Country,
            input.ZipCode);

        Result<UpdateLocationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UpdateLocationPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        return UpdateLocationPayload.Success($"Location '{result.Value.Name}' updated successfully.");
    }

    /// <summary>
    /// Changes location status.
    /// Requires locations:update permission.
    /// </summary>
    [Authorize(Policy = "locations:update")]
    public async Task<ChangeLocationStatusPayload> ChangeLocationStatusAsync(
        ChangeLocationStatusInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        LocationStatus locationStatus = input.Status switch
        {
            LocationStatusType.Active => LocationStatus.Active,
            LocationStatusType.Inactive => LocationStatus.Inactive,
            LocationStatusType.UnderMaintenance => LocationStatus.UnderMaintenance,
            LocationStatusType.Closed => LocationStatus.Closed,
            _ => LocationStatus.Active
        };

        var command = new ChangeLocationStatusCommand(
            input.LocationId,
            locationStatus);

        Result<ChangeLocationStatusResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ChangeLocationStatusPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        return ChangeLocationStatusPayload.Success($"Location status changed to {locationStatus} successfully.");
    }

    /// <summary>
    /// Assigns a user to a location.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<AssignUserToLocationPayload> AssignUserToLocationAsync(
        AssignUserToLocationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AssignUserToLocationCommand(
            input.UserId,
            input.LocationId,
            input.LocationRoleId,
            input.IsPrimaryLocation);

        Result<AssignUserToLocationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return AssignUserToLocationPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return AssignUserToLocationPayload.Success(
            result.Value.AssignmentId,
            result.Value.UserId,
            result.Value.LocationId,
            result.Value.Message);
    }

    /// <summary>
    /// Unassigns a user from a location.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<UnassignUserFromLocationPayload> UnassignUserFromLocationAsync(
        UnassignUserFromLocationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UnassignUserFromLocationCommand(
            input.UserId,
            input.LocationId);

        Result<UnassignUserFromLocationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UnassignUserFromLocationPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return UnassignUserFromLocationPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Switches a user's current location context.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<SwitchLocationContextPayload> SwitchLocationContextAsync(
        SwitchLocationContextInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new SwitchLocationContextCommand(
            input.UserId,
            input.LocationId);

        Result<SwitchLocationContextResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return SwitchLocationContextPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return SwitchLocationContextPayload.Success(
            result.Value.UserId,
            result.Value.NewLocationContextId,
            result.Value.Message);
    }

    /// <summary>
    /// Sets a user's primary location.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<SetPrimaryLocationPayload> SetPrimaryLocationAsync(
        SetPrimaryLocationInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new SetPrimaryLocationCommand(
            input.UserId,
            input.LocationId);

        Result<SetPrimaryLocationResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return SetPrimaryLocationPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return SetPrimaryLocationPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Assigns an owner to a location.
    /// Requires locations:update permission.
    /// </summary>
    [Authorize(Policy = "locations:update")]
    public async Task<AssignLocationOwnerPayload> AssignLocationOwnerAsync(
        AssignLocationOwnerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AssignLocationOwnerCommand(
            input.LocationId,
            input.UserId);

        Result<AssignLocationOwnerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return AssignLocationOwnerPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        return AssignLocationOwnerPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Removes the owner from a location.
    /// Requires locations:update permission.
    /// </summary>
    [Authorize(Policy = "locations:update")]
    public async Task<RemoveLocationOwnerPayload> RemoveLocationOwnerAsync(
        RemoveLocationOwnerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RemoveLocationOwnerCommand(input.LocationId);

        Result<RemoveLocationOwnerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RemoveLocationOwnerPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        return RemoveLocationOwnerPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Assigns a manager to a location.
    /// Requires locations:update permission.
    /// </summary>
    [Authorize(Policy = "locations:update")]
    public async Task<AssignLocationManagerPayload> AssignLocationManagerAsync(
        AssignLocationManagerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AssignLocationManagerCommand(
            input.LocationId,
            input.UserId);

        Result<AssignLocationManagerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return AssignLocationManagerPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        return AssignLocationManagerPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Removes the manager from a location.
    /// Requires locations:update permission.
    /// </summary>
    [Authorize(Policy = "locations:update")]
    public async Task<RemoveLocationManagerPayload> RemoveLocationManagerAsync(
        RemoveLocationManagerInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RemoveLocationManagerCommand(input.LocationId);

        Result<RemoveLocationManagerResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RemoveLocationManagerPayload.Failure(
                new LocationError(result.Error.Code, result.Error.Message));
        }

        return RemoveLocationManagerPayload.Success(result.Value.Message);
    }
}

