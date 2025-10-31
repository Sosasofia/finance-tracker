using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories(Guid userId);
    Task<bool> CategoryExistsAsync(Guid categoryId);
    Task<Category> AddCategoryAsync(Category customCategory);
    Task<bool> ExistsCategoryForUserAsync(Guid userId, string name);
}
