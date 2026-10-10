using FinanceTracker.Application.Features.Users.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Queries.GetUserPreferences;

public record GetUserPreferencesQuery : IRequest<UserPreferencesResponse>;
