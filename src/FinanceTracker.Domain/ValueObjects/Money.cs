using FinanceTracker.Domain.Exceptions;

namespace FinanceTracker.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpper();
    }

    public static Money Create(decimal amount, string currency = "ARS")
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required.");

        return new Money(amount, currency);
    }

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot add different currencies: {Currency} and {other.Currency}.");

        return new Money(Amount + other.Amount, Currency);
    }

    public override string ToString() => $"{Currency} {Amount:N2}";
}
