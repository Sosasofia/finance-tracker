using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(query.Id, query.userId);

        if (category == null)
        {
            throw new NotFoundException(nameof(CategoryDto), query.Id);
        }

        return CategoryDto.MapFrom(category);
    }
}
