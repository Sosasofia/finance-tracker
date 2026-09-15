using FinanceTracker.Domain.Exceptions;

namespace FinanceTracker.Domain.Entities;

public class PaymentMethod
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Type { get; private set; }
    public Guid? UserId { get; private set; }

    private PaymentMethod() { }

    public static PaymentMethod Create(string name, string type, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Payment method name is required.");
        if (string.IsNullOrWhiteSpace(type)) throw new DomainException("Payment method type is required.");

        return new PaymentMethod
        {
            Name = name,
            Type = type,
            UserId = userId
        };
    }

    public static PaymentMethod CreateDefault(string name, string type)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Payment method name is required.");
        if (string.IsNullOrWhiteSpace(type)) throw new DomainException("Payment method type is required.");

        return new PaymentMethod
        {
            Name = name,
            Type = type
        };
    }
}
