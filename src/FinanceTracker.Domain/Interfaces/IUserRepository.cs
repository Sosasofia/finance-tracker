namespace FinanceTracker.Domain.Repositories;

public interface IUserRepository
{
    Task<bool> ExistsAsync(Guid id);
}
