using Domain.Abstractions;
using MediatR;

namespace Application.Users.Queries.GetUsers;

/// <summary>
/// Query to get all users with pagination.
/// </summary>
public sealed record GetUsersQuery(
    int Page = 1,
    int PageSize = 10) : IRequest<Result<GetUsersResult, DomainError>>;

