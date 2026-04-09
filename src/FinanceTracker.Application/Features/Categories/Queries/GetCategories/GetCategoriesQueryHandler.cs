using AutoMapper;
using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesQueryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetCategoriesQuery query, CancellationToken ct)
    {
        var categories = await _categoryRepository.GetCategories(query.UserId);

        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }
}
