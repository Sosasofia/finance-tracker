namespace FinanceTracker.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQuery
{
    public Guid Id { get; init; }

    public GetCategoryByIdQuery(Guid id)
    {
        Id = id;
    }
}
