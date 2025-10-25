using Domain.Abstractions;
using MediatR;

namespace Application.Users.Queries.GetCurrentUser;

/// <summary>
/// Query to get the current authenticated user.
/// </summary>
public sealed record GetCurrentUserQuery(Guid UserId) : IRequest<Result<GetCurrentUserResult, DomainError>>;

