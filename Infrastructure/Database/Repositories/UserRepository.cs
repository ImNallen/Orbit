using Domain.Shared.ValueObjects;
using Domain.Users;
using Domain.Users.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

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
            .AsNoTracking()
            .OrderBy(u => u.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .CountAsync(cancellationToken);
    }

    public async Task<List<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetByLocationIdAsync(Guid locationId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.LocationAssignments.Any(a =>
                a.LocationId == locationId &&
                a.Status == Domain.Users.Enums.AssignmentStatus.Active))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
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

        // Handle location assignments separately since they're ignored in User configuration
        // Get existing assignments from database
        List<UserLocationAssignment> existingAssignments = await _context.UserLocationAssignments
            .Where(a => a.UserId == user.Id)
            .ToListAsync(cancellationToken);

        // Get current assignments from the user entity
        var currentAssignments = user.LocationAssignments.ToList();

        // Find assignments to add (in current but not in existing)
        var assignmentsToAdd = currentAssignments
            .Where(ca => !existingAssignments.Any(ea => ea.Id == ca.Id))
            .ToList();

        // Find assignments to update (in both, but may have changed)
        var assignmentsToUpdate = currentAssignments
            .Where(ca => existingAssignments.Any(ea => ea.Id == ca.Id))
            .ToList();

        // Find assignments to remove (in existing but not in current)
        var assignmentsToRemove = existingAssignments
            .Where(ea => !currentAssignments.Any(ca => ca.Id == ea.Id))
            .ToList();

        // Apply changes
        if (assignmentsToAdd.Any())
        {
            await _context.UserLocationAssignments.AddRangeAsync(assignmentsToAdd, cancellationToken);
        }

        if (assignmentsToUpdate.Any())
        {
            _context.UserLocationAssignments.UpdateRange(assignmentsToUpdate);
        }

        if (assignmentsToRemove.Any())
        {
            _context.UserLocationAssignments.RemoveRange(assignmentsToRemove);
        }

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
            .AsNoTracking()
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

