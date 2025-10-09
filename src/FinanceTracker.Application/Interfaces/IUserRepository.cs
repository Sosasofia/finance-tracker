using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface IUserRepository
{
    Task<bool> ExistsByIdAsync(Guid id);
    Task<User> ExistsByEmailAsync(string email);
    Task AddAsync(User user);
    Task<int> SaveChangesAsync();
}
