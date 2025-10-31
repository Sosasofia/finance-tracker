using FinanceTracker.Application.Features.Categories;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId);
    Task<CategoryDto> CreateCategoryAsync(Guid userId, CreateCategoryDto categoryDto);
}
