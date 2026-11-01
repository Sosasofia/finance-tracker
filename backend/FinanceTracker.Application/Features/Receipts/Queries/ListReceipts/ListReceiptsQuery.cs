using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Queries.ListReceipts;

public sealed record ListReceiptsQuery(Guid UserId) : IRequest<IReadOnlyList<ReceiptListItem>>;

public sealed record ReceiptListItem(
    string Id,
    string DisplayName,
    string OriginalFileName,
    string ContentType,
    long Size,
    DateTime UploadedAt,
    string? TransactionId);
