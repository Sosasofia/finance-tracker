using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Categories.Models;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery(Guid UserId) : IRequest<IEnumerable<CategoryDto>>;
