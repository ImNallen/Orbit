using Domain.Abstractions;
using MediatR;

namespace Application.Users.Queries.GetUsersByLocation;

/// <summary>
/// Query to get all users assigned to a specific location.
/// </summary>
public sealed record GetUsersByLocationQuery(
    Guid LocationId,
    int Page = 1,
    int PageSize = 10) : IRequest<Result<GetUsersByLocationResult, DomainError>>;

