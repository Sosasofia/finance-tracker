using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Queries.DownloadReceipt;

public sealed class DownloadReceiptQueryHandler : IRequestHandler<DownloadReceiptQuery, ReceiptDownloadResult>
{
    private readonly IReceiptStorageService _receiptStorage;

    public DownloadReceiptQueryHandler(IReceiptStorageService receiptStorage)
    {
        _receiptStorage = receiptStorage;
    }

    public async Task<ReceiptDownloadResult> Handle(
        DownloadReceiptQuery request,
        CancellationToken cancellationToken)
    {
        return await _receiptStorage.OpenReadAsync(
            request.ReceiptId,
            request.UserId,
            cancellationToken);
    }
}
