using Application.Users.Queries.GetUsers;
using Domain.Abstractions;
using Domain.Role;
using Domain.Users;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetUsersByLocation;

/// <summary>
/// Handler for GetUsersByLocationQuery.
/// </summary>
public sealed class GetUsersByLocationQueryHandler
    : IRequestHandler<GetUsersByLocationQuery, Result<GetUsersByLocationResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ILogger<GetUsersByLocationQueryHandler> _logger;

    public GetUsersByLocationQueryHandler(
        IUserRepository _userRepository,
        IRoleRepository roleRepository,
        ILogger<GetUsersByLocationQueryHandler> logger)
    {
        this._userRepository = _userRepository;
        _roleRepository = roleRepository;
        _logger = logger;
    }

    public async Task<Result<GetUsersByLocationResult, DomainError>> Handle(
        GetUsersByLocationQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Getting users for location {LocationId} - Page: {Page}, PageSize: {PageSize}",
            query.LocationId, query.Page, query.PageSize);

        // 1. Validate pagination parameters
        if (query.Page < 1)
        {
            return Result<GetUsersByLocationResult, DomainError>.Failure(ValidationErrors.InvalidPage);
        }

        if (query.PageSize < 1 || query.PageSize > 100)
        {
            return Result<GetUsersByLocationResult, DomainError>.Failure(ValidationErrors.InvalidPageSize);
        }

        // 2. Get all users assigned to the location
        List<User> allUsers = await _userRepository.GetByLocationIdAsync(query.LocationId, cancellationToken);
        int totalCount = allUsers.Count;

        // 3. Apply pagination
        int skip = (query.Page - 1) * query.PageSize;
        var users = allUsers
            .Skip(skip)
            .Take(query.PageSize)
            .ToList();

        // 4. Get all unique role IDs from users
        var roleIds = users.Select(u => u.RoleId).Distinct().ToList();

        // 5. Fetch all roles in one query
        List<Role> roles = await _roleRepository.GetByIdsAsync(roleIds, cancellationToken);
        var roleDict = roles.ToDictionary(r => r.Id, r => r.Name);

        // 6. Map to DTOs
        var userDtos = users.Select(u => new UserDto(
            u.Id,
            u.Email.Value,
            u.FullName.FirstName,
            u.FullName.LastName,
            u.IsEmailVerified,
            u.Status.ToString(),
            u.CreatedAt,
            u.LastLoginAt,
            roleDict.GetValueOrDefault(u.RoleId))).ToList();

        // 7. Calculate total pages
        int totalPages = (int)Math.Ceiling((double)totalCount / query.PageSize);

        _logger.LogDebug(
            "Retrieved {UserCount} users out of {TotalCount} total users for location {LocationId}",
            users.Count, totalCount, query.LocationId);

        return Result<GetUsersByLocationResult, DomainError>.Success(
            new GetUsersByLocationResult(
                userDtos,
                totalCount,
                query.Page,
                query.PageSize,
                totalPages));
    }
}

