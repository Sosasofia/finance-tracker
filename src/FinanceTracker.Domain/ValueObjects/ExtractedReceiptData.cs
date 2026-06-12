namespace FinanceTracker.Domain.ValueObjects;

public record ExtractedReceiptData
{
    public string? MerchantName { get; init; }
    public float? MerchantNameConfidence { get; init; }

    public double? TotalAmount { get; init; }
    public float? TotalAmountConfidence { get; init; }

    public string? TransactionDate { get; init; }
    public float? TransactionDateConfidence { get; init; }

    public string? PaymentType { get; init; }
    public float? PaymentTypeConfidence { get; init; }

    public string? ReceiptType { get; init; }
    public float? ReceiptTypeConfidence { get; init; }

    public string? RawContent { get; set; }

    public IReadOnlyList<string> LineItems { get; init; } = Array.Empty<string>();
}
