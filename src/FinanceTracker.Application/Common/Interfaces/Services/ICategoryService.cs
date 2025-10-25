using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
    Task<IEnumerable<CustomCategory>> GetCategoriesByUserIdAsync(Guid userId);
    Task<CustomCategory> CreateCustomCategoryAsync(Guid userId, CustomCategoryDto categoryDto);
}
