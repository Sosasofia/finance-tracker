using FinanceTracker.Application.Features.Categories.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(
    string Name) : IRequest<CategoryDto>
{
    public Guid UserId { get; init; }
}
