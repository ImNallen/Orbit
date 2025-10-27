using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.DeleteUser;

/// <summary>
/// Command to soft delete a user account.
/// </summary>
public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result<DeleteUserResult, DomainError>>;

/// <summary>
/// Result of deleting a user.
/// </summary>
public sealed record DeleteUserResult(string Message);

