using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands.UploadReceipt;

public sealed class UploadReceiptCommandHandler : IRequestHandler<UploadReceiptCommand, ReceiptStorageResult>
{
    private readonly IReceiptStorageService _receiptStorage;

    public UploadReceiptCommandHandler(IReceiptStorageService receiptStorage)
    {
        _receiptStorage = receiptStorage;
    }

    public Task<ReceiptStorageResult> Handle(UploadReceiptCommand request, CancellationToken cancellationToken)
    {
        return _receiptStorage.UploadAsync(
            request.FileStream,
            request.OriginalFileName,
            request.ContentType,
            request.UserId,
            cancellationToken);
    }
}
