using Domain.Abstractions;
using Domain.Role;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Roles.Queries.GetRoles;

/// <summary>
/// Handler for GetRolesQuery.
/// </summary>
public sealed class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, Result<GetRolesResult, DomainError>>
{
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<GetRolesQueryHandler> _logger;

    public GetRolesQueryHandler(
        IRoleRepository roleRepository,
        ILogger<GetRolesQueryHandler> logger)
    {
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<Result<GetRolesResult, DomainError>> Handle(
        GetRolesQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all roles");

        // Get all roles
        List<Role> roles = await _roleRepository.GetAllAsync(cancellationToken);

        // Map to DTOs
        var roleDtos = roles.Select(r => new RoleDto(
            r.Id,
            r.Name,
            r.Description,
            r.PermissionIds.ToList()
        )).ToList();

        _logger.LogInformation("Retrieved {RoleCount} roles", roles.Count);

        return Result<GetRolesResult, DomainError>.Success(
            new GetRolesResult(roleDtos));
    }
}

