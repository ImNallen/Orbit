using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.SetPrimaryLocation;

/// <summary>
/// Command to set a user's primary location.
/// </summary>
public sealed record SetPrimaryLocationCommand(
    Guid UserId,
    Guid LocationId) : IRequest<Result<SetPrimaryLocationResult, DomainError>>;

/// <summary>
/// Result of setting primary location.
/// </summary>
public sealed record SetPrimaryLocationResult(string Message);

