using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Users.Models;

namespace FinanceTracker.Application.Features.Users.Queries.GetUserPreferences;

public record GetUserPreferencesQuery : IRequest<UserPreferencesResponse>;
