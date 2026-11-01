using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Queries.ListReceipts;

public sealed class ListReceiptsQueryHandler : IRequestHandler<ListReceiptsQuery, IReadOnlyList<ReceiptListItem>>
{
    private readonly IReceiptRepository _repository;

    public ListReceiptsQueryHandler(IReceiptRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<ReceiptListItem>> Handle(
        ListReceiptsQuery request,
        CancellationToken cancellationToken)
    {
        var receipts = await _repository.GetByUserAsync(request.UserId, cancellationToken);
        return receipts.Select(receipt => new ReceiptListItem(
            receipt.StorageReceiptId,
            receipt.DisplayName,
            receipt.OriginalFileName,
            receipt.ContentType,
            receipt.Size,
            receipt.UploadedAt,
            receipt.TransactionId?.ToString())).ToList();
    }
}
