using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands.UploadReceipt;

public sealed class UploadReceiptCommandHandler : IRequestHandler<UploadReceiptCommand, ReceiptStorageResult>
{
    private readonly IReceiptStorageService _receiptStorage;
    private readonly IReceiptRepository _receiptRepository;

    public UploadReceiptCommandHandler(
        IReceiptStorageService receiptStorage,
        IReceiptRepository receiptRepository)
    {
        _receiptStorage = receiptStorage;
        _receiptRepository = receiptRepository;
    }

    public async Task<ReceiptStorageResult> Handle(UploadReceiptCommand request, CancellationToken cancellationToken)
    {
        var result = await _receiptStorage.UploadAsync(
            request.FileStream,
            request.OriginalFileName,
            request.ContentType,
            request.UserId,
            cancellationToken);

        var receipt = FinanceTracker.Domain.Entities.Receipt.Create(
            request.UserId,
            result.ReceiptId,
            request.DisplayName,
            request.OriginalFileName,
            result.ContentType,
            result.Size);
        await _receiptRepository.AddAsync(receipt, cancellationToken);
        await _receiptRepository.SaveChangesAsync(cancellationToken);
        return result;
    }
}
