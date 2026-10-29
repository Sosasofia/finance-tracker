namespace FinanceTracker.Application.DTOs;

public record ExtractedReceiptDto(
    string? TransactionDate,
    decimal? Amount,
    string? MerchantName,
    Guid? CategoryId,
    double? CategoryConfidence,
    Guid? PaymentMethodId,
    string? PaymentType,
    double? PaymentMethodConfidence,
    string? RawCategoryText,
    string? RawPaymentMethodText,
    string? RawContent,
    string[] LineItems
);
