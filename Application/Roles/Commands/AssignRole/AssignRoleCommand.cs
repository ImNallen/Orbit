using Domain.Abstractions;
using MediatR;

namespace Application.Roles.Commands.AssignRole;

/// <summary>
/// Command to assign a role to a user.
/// </summary>
public sealed record AssignRoleCommand(
    Guid UserId,
    Guid RoleId) : IRequest<Result<AssignRoleResult, DomainError>>;

