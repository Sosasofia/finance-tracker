using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId)
    {

        var categories = await _categoryRepository.GetCategories(userId);

        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }


    public async Task<CategoryDto> CreateCategoryAsync(Guid userId, CreateCategoryDto categoryDto)
    {
        var customCategoryExists = await _categoryRepository.ExistsCategoryForUserAsync(userId, categoryDto.Name);

        if (customCategoryExists)
        {
            throw new InvalidOperationException("A custom category with this name already exists for this user.");
        }

        var newCategoryEntity = _mapper.Map<Category>(categoryDto);

        newCategoryEntity.UserId = userId;
        newCategoryEntity.Type = CategoryType.Custom;

        var createdCategoryEntity = await _categoryRepository.AddCategoryAsync(newCategoryEntity);

        return _mapper.Map<CategoryDto>(createdCategoryEntity);
    }
}
