namespace FinanceTracker.Application.Common.Interfaces;

public interface IReceiptStorageService
{
    Task<ReceiptStorageResult> UploadAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        Guid userId,
        CancellationToken ct = default);

    Task<ReceiptDownloadResult> OpenReadAsync(
        string receiptId,
        Guid userId,
        CancellationToken ct = default);

    Task<ReceiptSecureUrlResult> GetSecureUrlAsync(
        string receiptId,
        Guid userId,
        CancellationToken ct = default);

    Task DeleteAsync(string receiptId, Guid userId, CancellationToken ct = default);
}

public sealed record ReceiptStorageResult(
    string ReceiptId,
    string ContentType,
    long Size,
    DateTimeOffset UploadedAt);

public sealed record ReceiptDownloadResult(
    Stream Content,
    string ContentType,
    string FileName,
    long? Length);

public sealed record ReceiptSecureUrlResult(
    string Url,
    DateTimeOffset ExpiresAt);
