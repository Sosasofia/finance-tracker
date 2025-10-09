using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface IUserApplicationService
{
    Task<bool> ExistsByIdAsync(Guid id);
    Task<User> FindOrCreateUserAsync(string email, string? name, string? pictureUrl);
}
