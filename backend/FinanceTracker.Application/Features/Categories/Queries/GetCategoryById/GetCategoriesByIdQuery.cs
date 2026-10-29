using FinanceTracker.Application.Features.Categories.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id, Guid userId) : IRequest<CategoryDto>;
