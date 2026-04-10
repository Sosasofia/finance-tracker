using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken ct)
    {
        var categories = await _categoryRepository.GetCategories(query.UserId);

        return categories.Select(CategoryDto.MapFrom);
    }
}
