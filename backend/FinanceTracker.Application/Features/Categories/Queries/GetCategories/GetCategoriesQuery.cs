using FinanceTracker.Application.Features.Categories.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(Guid UserId) : IRequest<IEnumerable<CategoryDto>>;
