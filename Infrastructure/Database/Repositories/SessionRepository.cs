using Domain.Session;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories;

/// <summary>
/// Repository implementation for Session aggregate.
/// </summary>
public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Session?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.RefreshToken == refreshToken, cancellationToken);
    }

    public async Task<List<Session>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsRevoked && s.ExpiresAt > now)
            .OrderByDescending(s => s.LastAccessedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Session>> GetAllSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Session>> GetExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        return await _context.Sessions
            .AsNoTracking()
            .Where(s => s.ExpiresAt <= now && !s.IsRevoked)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default)
    {
        await _context.Sessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Session session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Session session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        List<Session> sessions = await _context.Sessions
            .Where(s => s.UserId == userId)
            .ToListAsync(cancellationToken);

        _context.Sessions.RemoveRange(sessions);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteExpiredSessionsAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        List<Session> expiredSessions = await _context.Sessions
            .Where(s => s.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        _context.Sessions.RemoveRange(expiredSessions);
        await _context.SaveChangesAsync(cancellationToken);

        return expiredSessions.Count;
    }
}

