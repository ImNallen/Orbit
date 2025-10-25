using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.Role;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Roles.Commands.AssignRole;

/// <summary>
/// Handler for AssignRoleCommand.
/// </summary>
public sealed class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result<AssignRoleResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<AssignRoleCommandHandler> _logger;

    public AssignRoleCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<AssignRoleCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<Result<AssignRoleResult, DomainError>> Handle(
        AssignRoleCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Assigning role {RoleId} to user {UserId}", command.RoleId, command.UserId);

        // 1. Get the user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<AssignRoleResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Get the role
        Role? role = await _roleRepository.GetByIdAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            _logger.LogWarning("Role {RoleId} not found", command.RoleId);
            return Result<AssignRoleResult, DomainError>.Failure(UserErrors.RoleNotFound);
        }

        // 3. Assign the role to the user
        Result<DomainError> assignResult = user.AssignRole(role.Id, role.Name);
        if (assignResult.IsFailure)
        {
            _logger.LogWarning("Failed to assign role {RoleId} to user {UserId}: {Error}",
                command.RoleId, command.UserId, assignResult.Error.Message);
            return Result<AssignRoleResult, DomainError>.Failure(assignResult.Error);
        }

        // 4. Save the user
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Successfully assigned role {RoleName} to user {UserId}", role.Name, command.UserId);

        return Result<AssignRoleResult, DomainError>.Success(
            new AssignRoleResult($"Role '{role.Name}' assigned successfully"));
    }
}

