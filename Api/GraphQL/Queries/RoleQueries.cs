using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Permissions.Queries.GetPermissions;
using Application.Roles.Queries.GetRoles;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for roles and permissions.
/// </summary>
[ExtendObjectType("Query")]
public sealed class RoleQueries
{
    /// <summary>
    /// Get all roles.
    /// Requires roles:read permission.
    /// </summary>
    [Authorize(Policy = "roles:read")]
    public async Task<RolesPayload> RolesAsync(
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetRolesQuery();
        Result<GetRolesResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return RolesPayload.Failure(new UserError(result.Error.Code, result.Error.Message));
        }

        // Map to GraphQL types
        var roles = result.Value.Roles.Select(r => new RoleType
        {
            RoleId = r.RoleId,
            Name = r.Name,
            Description = r.Description,
            PermissionIds = r.PermissionIds
        }).ToList();

        return RolesPayload.Success(roles);
    }

    /// <summary>
    /// Get all permissions.
    /// Requires permissions:read permission.
    /// </summary>
    [Authorize(Policy = "permissions:read")]
    public async Task<PermissionsPayload> PermissionsAsync(
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetPermissionsQuery();
        Result<GetPermissionsResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return PermissionsPayload.Failure(new UserError(result.Error.Code, result.Error.Message));
        }

        // Map to GraphQL types
        var permissions = result.Value.Permissions.Select(p => new PermissionType
        {
            PermissionId = p.PermissionId,
            Name = p.Name,
            Description = p.Description,
            Resource = p.Resource,
            Action = p.Action
        }).ToList();

        return PermissionsPayload.Success(permissions);
    }
}

