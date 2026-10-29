using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IUserPreferencesRepository
{
    Task<UserPreferences?> GetByUserIdAsync(Guid userId);
    Task AddAsync(UserPreferences preferences);
    Task UpdateAsync(UserPreferences preferences);
}
