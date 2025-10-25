using Domain.Users.User;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repository implementation for User aggregate.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task<User?> GetByEmailVerificationTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.EmailVerificationToken == token, cancellationToken);
    }

    public async Task<User?> GetByPasswordResetTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.PasswordResetToken == token, cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(int skip = 0, int take = 100, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OrderBy(u => u.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.CountAsync(cancellationToken);
    }

    public async Task<List<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .Where(u => u.RoleIds.Contains(roleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<PasswordHistory>> GetPasswordHistoryAsync(Guid userId, int limit = 5, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordHistory
            .Where(ph => ph.UserId == userId)
            .OrderByDescending(ph => ph.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task AddPasswordHistoryAsync(PasswordHistory passwordHistory, CancellationToken cancellationToken = default)
    {
        await _context.PasswordHistory.AddAsync(passwordHistory, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

