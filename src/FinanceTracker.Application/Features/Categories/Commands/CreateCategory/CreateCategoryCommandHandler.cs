using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var exists = await _categoryRepository.ExistsForUserAsync(command.UserId, command.Name, ct);
        if (exists)
        {
            throw new InvalidOperationException("A custom category with this name already exists.");
        }

        var category = Category.Create(command.Name, CategoryType.Custom, command.UserId);
        var createdCategory = await _categoryRepository.AddAsync(category, ct);

        return CategoryDto.MapFrom(createdCategory);
    }
}
