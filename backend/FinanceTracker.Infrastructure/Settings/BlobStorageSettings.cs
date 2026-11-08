namespace FinanceTracker.Infrastructure.Settings;

public class BlobStorageSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public string AccountUrl { get; init; } = string.Empty;
    public string ContainerName { get; init; } = string.Empty;
    public long MaxFileSizeBytes { get; init; } = 5 * 1024 * 1024;
    public int OrphanRetentionDays { get; init; } = 7;
    public int SasLifetimeMinutes { get; init; } = 15;
}
