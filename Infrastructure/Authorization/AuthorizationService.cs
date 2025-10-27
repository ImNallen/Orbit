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
        // 1. Get user to access their role ID
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return [];
        }

        // 2. Load role from database
        Role? role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        if (role is null)
        {
            return [];
        }

        // 3. Return role name
        return [role.Name];
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 1. Get user to access their role ID
        User? user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return [];
        }

        // 2. Load role from database
        Role? role = await _roleRepository.GetByIdAsync(user.RoleId, cancellationToken);
        if (role is null)
        {
            return [];
        }

        // 3. Get permission IDs from the role
        var permissionIds = role.PermissionIds.ToList();

        // 4. If no permissions, return empty list
        if (!permissionIds.Any())
        {
            return [];
        }

        // 5. Load permissions from database
        List<Permission> permissions = await _permissionRepository.GetByIdsAsync(permissionIds, cancellationToken);

        // 6. Extract permission names
        return permissions.Select(p => p.Name).ToList();
    }
}

