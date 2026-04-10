using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    [Required(ErrorMessage = "The category name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 50 characters.")]
    string Name)
{
    public Guid UserId { get; init; }
}
