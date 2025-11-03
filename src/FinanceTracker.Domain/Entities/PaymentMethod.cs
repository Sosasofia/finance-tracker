namespace FinanceTracker.Domain.Entities;

public class PaymentMethod
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public Guid? UserId { get; set; }
}
