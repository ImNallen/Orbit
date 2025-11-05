using Api.GraphQL.Inputs;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Locations.Commands.CreateLocation;
using Application.Locations.Commands.UpdateLocation;
using Application.Locations.Commands.ChangeLocationStatus;
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
}

