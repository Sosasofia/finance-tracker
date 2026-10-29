using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Users.Models;

namespace FinanceTracker.Application.Features.Users.Commands.UpdateUserPreferences;

public record UpdateUserPreferencesCommand(string DefaultCurrency, string DateFormat) : IRequest<UserPreferencesResponse>;
