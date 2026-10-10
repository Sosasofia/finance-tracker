using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Infrastructure.Repositories;

public class UserPreferencesRepository : IUserPreferencesRepository
{
    private readonly ApplicationDbContext _context;

    public UserPreferencesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserPreferences?> GetByUserIdAsync(Guid userId)
    {
        return await _context.UserPreferences
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task AddAsync(UserPreferences preferences)
    {
        _context.UserPreferences.Add(preferences);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserPreferences preferences)
    {
        _context.UserPreferences.Update(preferences);
        await _context.SaveChangesAsync();
    }
}
