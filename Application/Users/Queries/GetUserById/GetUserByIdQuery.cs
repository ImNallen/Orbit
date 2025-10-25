using Domain.Abstractions;
using MediatR;

namespace Application.Users.Queries.GetUserById;

/// <summary>
/// Query to get a user by their ID.
/// </summary>
public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<GetUserByIdResult, DomainError>>;

