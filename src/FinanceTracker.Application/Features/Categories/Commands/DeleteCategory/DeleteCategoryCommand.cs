namespace FinanceTracker.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid CategoryId, Guid UserId);
