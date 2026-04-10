using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var exists = await _categoryRepository.ExistsForUserAsync(command.UserId, command.Name);
        if (exists)
        {
            throw new InvalidOperationException("A custom category with this name already exists.");
        }

        var category = Category.Create(command.Name, CategoryType.Custom, command.UserId);
        var createdCategory = await _categoryRepository.AddAsync(category);

        return CategoryDto.MapFrom(createdCategory);
    }
}
