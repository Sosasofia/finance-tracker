using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories(Guid userId, CancellationToken ct);
    Task<bool> ExistsAsync(Guid categoryId, CancellationToken ct);
    Task<Category> AddAsync(Category customCategory, CancellationToken ct);
    Task<bool> ExistsForUserAsync(Guid userId, string name, CancellationToken ct);
    Task<Category?> GetByIdAsync(Guid categoryId, Guid userId, CancellationToken ct);
    Task<bool> IsInUseAsync(Guid categoryId, CancellationToken ct);
    Task DeleteAsync(Category category, CancellationToken ct);
}
