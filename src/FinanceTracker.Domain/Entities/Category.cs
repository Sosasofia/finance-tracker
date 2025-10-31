using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class Category
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public CategoryType Type { get; set; } = CategoryType.Standard;
    public Guid? UserId { get; set; }
}
