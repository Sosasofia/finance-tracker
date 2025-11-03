using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
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

    public async Task<CategoryDto> GetByIdAsync(Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId)
            ?? throw new NotFoundException(nameof(CategoryDto), categoryId);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> CreateAsync(Guid userId, CreateCategoryDto categoryDto)
    {
        if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
        {
            throw new InvalidOperationException("Invalid payment method data.");
        }

        var customCategoryExists = await _categoryRepository.ExistsForUserAsync(userId, categoryDto.Name);

        if (customCategoryExists)
        {
            throw new InvalidOperationException("A custom category with this name already exists for this user.");
        }

        var newCategoryEntity = _mapper.Map<Category>(categoryDto);

        newCategoryEntity.UserId = userId;
        newCategoryEntity.Type = CategoryType.Custom;

        var createdCategoryEntity = await _categoryRepository.AddAsync(newCategoryEntity);

        return _mapper.Map<CategoryDto>(createdCategoryEntity);
    }

    public async Task DeleteAsync(Guid userId, Guid categoryId)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId) ?? throw new NotFoundException(nameof(categoryId), categoryId);

        if (category.Type != CategoryType.Custom || category.UserId != userId)
        {
            throw new ForbiddenAccessException("You can only delete your own custom categories.");
        }

        var inUse = await _categoryRepository.IsInUseAsync(categoryId);

        if (inUse)
        {
            throw new InvalidOperationException("Cannot delete category because it is referenced by transactions.");
        }

        await _categoryRepository.DeleteAsync(category);
    }
}
