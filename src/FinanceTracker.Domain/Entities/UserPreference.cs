using FinanceTracker.Domain.Exceptions;

namespace FinanceTracker.Domain.Entities;

public class UserPreferences
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string DefaultCurrency { get; private set; } = "ARS";
    public string DateFormat { get; private set; } = "MM/dd/yyyy";
    public User User { get; private set; }

    private UserPreferences() { }

    public static UserPreferences CreateDefault(Guid userId)
    {
        return new UserPreferences
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DefaultCurrency = "ARS",
            DateFormat = "dd/MM/yyyy"
        };
    }

    public void Update(string defaultCurrency, string dateFormat)
    {
        if (string.IsNullOrWhiteSpace(defaultCurrency))
        {
            throw new DomainException("Default currency is required.");
        }

        if (string.IsNullOrWhiteSpace(dateFormat))
        {
            throw new DomainException("Date format is required.");
        }

        DefaultCurrency = defaultCurrency;
        DateFormat = dateFormat;
    }
}
