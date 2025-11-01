using Domain.Abstractions;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetUsers;

/// <summary>
/// Handler for GetUsersQuery.
/// </summary>
public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, Result<GetUsersResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUsersQueryHandler> _logger;

    public GetUsersQueryHandler(
        IUserRepository userRepository,
        ILogger<GetUsersQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<GetUsersResult, DomainError>> Handle(
        GetUsersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting users - Page: {Page}, PageSize: {PageSize}", query.Page, query.PageSize);

        // 1. Validate pagination parameters
        if (query.Page < 1)
        {
            return Result<GetUsersResult, DomainError>.Failure(ValidationErrors.InvalidPage);
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            return Result<GetUsersResult, DomainError>.Failure(ValidationErrors.InvalidPageSize);
        }

        // 2. Calculate skip and take
        int skip = (query.Page - 1) * query.PageSize;
        int take = query.PageSize;

        // 3. Get total count
        int totalCount = await _userRepository.GetCountAsync(cancellationToken);

        // 4. Get users
        List<User> users = await _userRepository.GetAllAsync(skip, take, cancellationToken);

        // 5. Map to DTOs
        var userDtos = users.Select(u => new UserDto(
            u.Id,
            u.Email.Value,
            u.FullName.FirstName,
            u.FullName.LastName,
            u.IsEmailVerified,
            u.Status.ToString(),
            u.CreatedAt,
            u.LastLoginAt)).ToList();

        // 6. Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        _logger.LogDebug("Retrieved {UserCount} users out of {TotalCount} total users",
            users.Count, totalCount);

        return Result<GetUsersResult, DomainError>.Success(
            new GetUsersResult(
                userDtos,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages));
    }
}

