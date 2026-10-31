using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid categoryId, CancellationToken ct = default);
    Task<Category> AddAsync(Category customCategory, CancellationToken ct = default);
    Task<bool> ExistsForUserAsync(Guid userId, string name, CancellationToken ct = default);
    Task<Category?> GetByIdAsync(Guid categoryId, Guid userId, CancellationToken ct = default);
    Task<bool> IsInUseAsync(Guid categoryId, CancellationToken ct = default);
    Task DeleteAsync(Category category, CancellationToken ct = default);
}
