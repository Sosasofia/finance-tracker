using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Repositories;

public interface ICategoryRepository
{
    Task<IEnumerable<Category>> GetCategories();
    Task<bool> CategoryExistsAsync(Guid categoryId);
    Task<CustomCategory> AddCustomCategoryAsync(CustomCategory customCategory);
    Task<IEnumerable<CustomCategory>> GetCustomCategoriesAsync(Guid userId);
    Task<bool> CategoryExistsForUserAsync(Guid userId, string name);
}
