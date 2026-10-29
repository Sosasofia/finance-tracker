using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.Users.Models;

public record UserPreferencesResponse(
    string DefaultCurrency,
    string DateFormat
)
{
    public static UserPreferencesResponse MapFrom(UserPreferences p) => new(
        DefaultCurrency: p.DefaultCurrency,
        DateFormat: p.DateFormat
    );
}
