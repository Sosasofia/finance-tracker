namespace FinanceTracker.Application.Features.Categories;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = String.Empty;
    public string Type { get; set; } = "Default";
    public Guid? UserId { get; set; }
}
