using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Categories.Commands.DeleteCategory;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, command.UserId, ct)
            ?? throw new NotFoundException(nameof(Category), command.CategoryId);

        if (category.Type != CategoryType.Custom)
        {
            throw new ForbiddenAccessException("System categories cannot be deleted.");
        }

        var inUse = await _categoryRepository.IsInUseAsync(command.CategoryId, ct);
        if (inUse)
        {
            throw new InUseException(command.CategoryId, "category");
        }

        await _categoryRepository.DeleteAsync(category, ct);

        return Unit.Value;
    }
}
