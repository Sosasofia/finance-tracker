using System.Diagnostics.Metrics;

namespace FinanceTracker.Server.Metrics;

public static class ApiMetrics
{
    public const string MeterName = "FinanceTracker.Api";
    private static readonly Meter Meter = new(MeterName, "1.0.0");

    public static readonly Counter<long> RateLimitExceededCounter =
        Meter.CreateCounter<long>(
            name: "rate_limits.exceeded.total",
            unit: "{request}",
            description: "Total number of rejected requests due to rate limiting");
}
