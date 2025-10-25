using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.Role;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Roles.Commands.RemoveRole;

/// <summary>
/// Handler for RemoveRoleCommand.
/// </summary>
public sealed class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, Result<RemoveRoleResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<RemoveRoleCommandHandler> _logger;

    public RemoveRoleCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        ILogger<RemoveRoleCommandHandler> logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<Result<RemoveRoleResult, DomainError>> Handle(
        RemoveRoleCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Removing role {RoleId} from user {UserId}", command.RoleId, command.UserId);

        // 1. Get the user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<RemoveRoleResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Get the role
        Role? role = await _roleRepository.GetByIdAsync(command.RoleId, cancellationToken);
        if (role is null)
        {
            _logger.LogWarning("Role {RoleId} not found", command.RoleId);
            return Result<RemoveRoleResult, DomainError>.Failure(UserErrors.RoleNotFound);
        }

        // 3. Remove the role from the user
        Result<DomainError> removeResult = user.RevokeRole(role.Id, role.Name);
        if (removeResult.IsFailure)
        {
            _logger.LogWarning("Failed to remove role {RoleId} from user {UserId}: {Error}",
                command.RoleId, command.UserId, removeResult.Error.Message);
            return Result<RemoveRoleResult, DomainError>.Failure(removeResult.Error);
        }

        // 4. Save the user
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Successfully removed role {RoleName} from user {UserId}", role.Name, command.UserId);

        return Result<RemoveRoleResult, DomainError>.Success(
            new RemoveRoleResult($"Role '{role.Name}' removed successfully"));
    }
}

