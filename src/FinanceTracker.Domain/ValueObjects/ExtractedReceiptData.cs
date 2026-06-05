namespace FinanceTracker.Domain.ValueObjects;

public class ExtractedReceiptData
{
    public string? MerchantName { get; set; }
    public double? TotalAmount { get; set; }
    public string? TransactionDate { get; set; }
}
