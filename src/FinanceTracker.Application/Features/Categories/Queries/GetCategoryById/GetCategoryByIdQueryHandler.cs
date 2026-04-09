using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery query, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(query.Id);

        if (category == null)
        {
            throw new NotFoundException(nameof(CategoryDto), query.Id);
        }

        return _mapper.Map<CategoryDto>(category);
    }
}
