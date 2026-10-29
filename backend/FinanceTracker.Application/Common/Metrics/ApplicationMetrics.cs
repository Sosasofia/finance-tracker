using System.Diagnostics.Metrics;

namespace FinanceTracker.Application.Common.Metrics;

public static class ApplicationMetrics
{
    public const string MeterName = "FinanceTracker.Metrics";
    private static readonly Meter Meter = new(MeterName, "1.0.0");

    public static readonly Counter<long> ReceiptsScannedCounter =
        Meter.CreateCounter<long>(
            name: "receipts.scanned.total",
            unit: "{receipt}",
            description: "Total number of receipts processed by OCR");

    public static readonly Histogram<double> ReceiptAmountHistogram =
        Meter.CreateHistogram<double>(
            name: "receipts.parsed_amount",
            unit: "ARS",
            description: "Monetary amount parsed from processed receipts");
}
