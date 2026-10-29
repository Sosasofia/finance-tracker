using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Categories.Models;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id, Guid userId) : IRequest<CategoryDto>;
