using FinanceTracker.Domain.Entities;
using FinanceTracker.Server.Models.DTOs;

namespace FinanceTracker.Server.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetCategoriesAsync(); 
    Task<IEnumerable<CustomCategory>> GetCategoriesByUserIdAsync(Guid userId);
    Task<CustomCategory> CreateCustomCategoryAsync(Guid userId, CustomCategoryDTO categoryDTO);
}
