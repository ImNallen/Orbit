using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetUserById;

/// <summary>
/// Handler for GetUserByIdQuery.
/// </summary>
public sealed class GetUserByIdQueryHandler
    : IRequestHandler<GetUserByIdQuery, Result<GetUserByIdResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserByIdQueryHandler> _logger;

    public GetUserByIdQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUserByIdQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<GetUserByIdResult, DomainError>> Handle(
        GetUserByIdQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", query.UserId);

        // 1. Get user by ID
        User? user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", query.UserId);
            return Result<GetUserByIdResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        _logger.LogDebug("User {UserId} found: {Email}", user.Id, user.Email.Value);

        // 2. Return result
        return Result<GetUserByIdResult, DomainError>.Success(
            new GetUserByIdResult(
                user.Id,
                user.Email.Value,
                user.FullName.FirstName,
                user.FullName.LastName,
                user.IsEmailVerified,
                user.Status.ToString(),
                user.CreatedAt,
                user.LastLoginAt));
    }
}

