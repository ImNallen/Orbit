using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.VerifyEmail;

/// <summary>
/// Command to verify a user's email address.
/// </summary>
public sealed record VerifyEmailCommand(
    string Token) : IRequest<Result<VerifyEmailResult, DomainError>>;

/// <summary>
/// Result of email verification.
/// </summary>
public sealed record VerifyEmailResult(
    Guid UserId,
    string Email,
    bool Success);

