using Domain.Abstractions;
using MediatR;

namespace Application.Roles.Queries.GetRoles;

/// <summary>
/// Query to get all roles.
/// </summary>
public sealed record GetRolesQuery : IRequest<Result<GetRolesResult, DomainError>>;

