using FinanceTracker.Application.Features.Categories;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId);
    Task<CategoryDto> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto categoryDto);
    Task DeleteAsync(Guid userId, Guid categoryId);
}
