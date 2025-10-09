using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Interfaces;

public interface ICategoryApplicationService
{
    Task<IEnumerable<Category>> GetCategoriesAsync();
    Task<IEnumerable<CustomCategory>> GetCategoriesByUserIdAsync(Guid userId);
    Task<CustomCategory> CreateCustomCategoryAsync(Guid userId, CustomCategoryDto categoryDTO);
}
