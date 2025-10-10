using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync(); 
        Task<IEnumerable<CustomCategory>> GetCategoriesByUserIdAsync(Guid userId);
        Task<CustomCategory> CreateCustomCategoryAsync(Guid userId, CustomCategoryDTO categoryDTO);
    }
}
