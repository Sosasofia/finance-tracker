using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User> FindByEmailAsync(string email);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task<int> SaveChangesAsync();
}
