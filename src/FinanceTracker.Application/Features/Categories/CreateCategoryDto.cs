using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Categories;

public class CreateCategoryDto
{
    [MaxLength(15)]
    public string Name { get; set; } = String.Empty;
}
