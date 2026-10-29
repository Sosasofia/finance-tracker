using FinanceTracker.Application.Features.Users.Models;
using MediatR;

namespace FinanceTracker.Application.Features.Users.Commands.UpdateUserPreferences;

public record UpdateUserPreferencesCommand(string DefaultCurrency, string DateFormat) : IRequest<UserPreferencesResponse>;
