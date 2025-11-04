namespace FinanceTracker.Application.Features.PaymentMethods;

public class CreatePaymentMethodDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
