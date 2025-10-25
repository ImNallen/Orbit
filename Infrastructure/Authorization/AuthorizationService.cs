using Domain.Abstractions;
using Domain.Users.Permission;
using Domain.Users.Role;
using Domain.Users.User;

namespace Infrastructure.Authorization;

/// <summary>
/// Service for loading user authorization data (roles and permissions).
/// </summary>
public class AuthorizationService : IAuthorizationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;

    public AuthorizationService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 1. Get user to access their role IDs
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return [];
        }

        // 2. If user has no roles, return empty list
        if (!user.RoleIds.Any())
        {
            return [];
        }

        // 3. Load roles from database
        List<Role> roles = await _roleRepository.GetByIdsAsync(user.RoleIds, cancellationToken);

        // 4. Extract role names
        return roles.Select(r => r.Name).ToList();
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 1. Get user to access their role IDs
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return [];
        }

        // 2. If user has no roles, return empty list
        if (!user.RoleIds.Any())
        {
            return [];
        }

        // 3. Load roles from database
        List<Role> roles = await _roleRepository.GetByIdsAsync(user.RoleIds, cancellationToken);

        // 4. Aggregate all permission IDs from all roles (distinct)
        var permissionIds = roles
            .SelectMany(r => r.PermissionIds)
            .Distinct()
            .ToList();

        // 5. If no permissions, return empty list
        if (!permissionIds.Any())
        {
            return [];
        }

        // 6. Load permissions from database
        List<Permission> permissions = await _permissionRepository.GetByIdsAsync(permissionIds, cancellationToken);

        // 7. Extract permission names
        return permissions.Select(p => p.Name).ToList();
    }
}

