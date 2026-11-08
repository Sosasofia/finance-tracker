using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Queries.DownloadReceipt;

public sealed record DownloadReceiptQuery(string ReceiptId, Guid UserId) : IRequest<ReceiptDownloadResult>;
