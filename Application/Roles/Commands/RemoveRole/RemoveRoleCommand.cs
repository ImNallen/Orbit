using Domain.Abstractions;
using MediatR;

namespace Application.Roles.Commands.RemoveRole;

/// <summary>
/// Command to remove a role from a user.
/// </summary>
public sealed record RemoveRoleCommand(
    Guid UserId,
    Guid RoleId) : IRequest<Result<RemoveRoleResult, DomainError>>;

