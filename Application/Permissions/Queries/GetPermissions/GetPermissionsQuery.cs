using Domain.Abstractions;
using MediatR;

namespace Application.Permissions.Queries.GetPermissions;

/// <summary>
/// Query to get all permissions.
/// </summary>
public sealed record GetPermissionsQuery : IRequest<Result<GetPermissionsResult, DomainError>>;

