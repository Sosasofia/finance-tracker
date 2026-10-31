using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands.DeleteReceipt;

public sealed class DeleteReceiptCommandHandler : IRequestHandler<DeleteReceiptCommand, Unit>
{
    private readonly IReceiptStorageService _receiptStorage;

    public DeleteReceiptCommandHandler(IReceiptStorageService receiptStorage)
    {
        _receiptStorage = receiptStorage;
    }

    public async Task<Unit> Handle(DeleteReceiptCommand request, CancellationToken cancellationToken)
    {
        await _receiptStorage.DeleteAsync(request.ReceiptId, request.UserId, cancellationToken);

        return Unit.Value;
    }
}
