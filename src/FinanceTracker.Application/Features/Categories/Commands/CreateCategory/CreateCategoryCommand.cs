using System.ComponentModel.DataAnnotations;
using FinanceTracker.Application.Features.Categories.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    [Required(ErrorMessage = "The category name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 50 characters.")]
    string Name) : IRequest<CategoryDto>
{
    public Guid UserId { get; init; }
}
