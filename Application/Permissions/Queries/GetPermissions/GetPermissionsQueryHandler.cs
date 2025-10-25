using Domain.Abstractions;
using Domain.Users.Permission;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Permissions.Queries.GetPermissions;

/// <summary>
/// Handler for GetPermissionsQuery.
/// </summary>
public sealed class GetPermissionsQueryHandler : IRequestHandler<GetPermissionsQuery, Result<GetPermissionsResult, DomainError>>
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly ILogger<GetPermissionsQueryHandler> _logger;

    public GetPermissionsQueryHandler(
        IPermissionRepository permissionRepository,
        ILogger<GetPermissionsQueryHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    public async Task<Result<GetPermissionsResult, DomainError>> Handle(
        GetPermissionsQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all permissions");

        // Get all permissions
        List<Permission> permissions = await _permissionRepository.GetAllAsync(cancellationToken);

        // Map to DTOs
        var permissionDtos = permissions.Select(p => new PermissionDto(
            p.Id,
            p.Name,
            p.Description,
            p.Resource,
            p.Action
        )).ToList();

        _logger.LogInformation("Retrieved {PermissionCount} permissions", permissions.Count);

        return Result<GetPermissionsResult, DomainError>.Success(
            new GetPermissionsResult(permissionDtos));
    }
}

