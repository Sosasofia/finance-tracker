using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public CategoryType Type { get; private set; } 
    public Guid? UserId { get; private set; }

    private Category() { }

    public static Category Create(string name, CategoryType type, Guid? userId = null)
    {
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException("Category name cannot be null");

        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Type = type,
            UserId = userId
        };
    }
}
