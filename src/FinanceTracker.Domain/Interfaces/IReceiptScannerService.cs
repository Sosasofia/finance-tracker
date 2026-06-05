using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Domain.Interfaces;

public interface IReceiptScannerService
{
    Task<ExtractedReceiptData> ScanReceiptAsync(Stream receiptStream);
}
