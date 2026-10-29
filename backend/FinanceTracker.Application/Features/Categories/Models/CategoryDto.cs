using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Categories.Models;

public record CategoryDto(
    Guid Id,
    string Name,
    string Type)
{
    public static CategoryDto MapFrom(Category category) => new(
        category.Id,
        category.Name,
        category.Type.ToString()
    );
}
