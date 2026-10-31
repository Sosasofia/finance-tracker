using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands.UploadReceipt;

public sealed record UploadReceiptCommand(
    Stream FileStream,
    string OriginalFileName,
    string ContentType,
    long FileLength,
    Guid UserId) : IRequest<ReceiptStorageResult>;
