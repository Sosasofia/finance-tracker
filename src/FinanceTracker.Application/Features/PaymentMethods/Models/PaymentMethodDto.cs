namespace FinanceTracker.Application.Features.PaymentMethods.Models;

public class PaymentMethodDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public Guid? UserId { get; init; }
}
