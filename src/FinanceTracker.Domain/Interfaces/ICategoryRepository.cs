using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories(Guid userId);
    Task<bool> ExistsAsync(Guid categoryId);
    Task<Category> AddAsync(Category customCategory);
    Task<bool> ExistsForUserAsync(Guid userId, string name);
    Task<Category?> GetByIdAsync(Guid categoryId);
    Task<bool> IsInUseAsync(Guid categoryId);
    Task DeleteAsync(Category category);
}
