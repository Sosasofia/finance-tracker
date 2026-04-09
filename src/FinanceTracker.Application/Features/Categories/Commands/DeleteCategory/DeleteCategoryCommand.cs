namespace FinanceTracker.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand
{
    public Guid CategoryId { get; init; }
    public Guid UserId { get; init; }

    public DeleteCategoryCommand(Guid categoryId, Guid userId)
    {
        CategoryId = categoryId;
        UserId = userId;
    }
}
