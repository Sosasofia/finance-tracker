using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Categories.Commands.DeleteCategory;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

public class DeleteCategoryCommandHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, command.UserId)
            ?? throw new NotFoundException(nameof(Category), command.CategoryId);

        if (category.Type != CategoryType.Custom)
        {
            throw new ForbiddenAccessException("System categories cannot be deleted.");
        }

        var inUse = await _categoryRepository.IsInUseAsync(command.CategoryId);
        if (inUse)
        {
            throw new InvalidOperationException("Cannot delete category because it is referenced by transactions.");
        }

        await _categoryRepository.DeleteAsync(category);
    }
}
