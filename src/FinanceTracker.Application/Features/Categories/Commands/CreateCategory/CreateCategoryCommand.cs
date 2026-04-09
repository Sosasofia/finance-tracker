using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommand
{
    [Required(ErrorMessage = "The category name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 50 characters.")]
    public required string Name { get; set; }
    public Guid UserId { get; set; }
}
