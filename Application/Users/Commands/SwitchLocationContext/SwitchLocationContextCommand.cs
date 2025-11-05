using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.SwitchLocationContext;

/// <summary>
/// Command to switch a user's current location context.
/// </summary>
public sealed record SwitchLocationContextCommand(
    Guid UserId,
    Guid LocationId) : IRequest<Result<SwitchLocationContextResult, DomainError>>;

/// <summary>
/// Result of switching location context.
/// </summary>
public sealed record SwitchLocationContextResult(
    Guid UserId,
    Guid NewLocationContextId,
    string Message);

