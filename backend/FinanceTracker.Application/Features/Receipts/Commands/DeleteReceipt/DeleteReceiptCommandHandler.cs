using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands.DeleteReceipt;

public sealed class DeleteReceiptCommandHandler : IRequestHandler<DeleteReceiptCommand, Unit>
{
    private readonly IReceiptStorageService _receiptStorage;
    private readonly IReceiptRepository _repository;

    public DeleteReceiptCommandHandler(
        IReceiptStorageService receiptStorage,
        IReceiptRepository repository)
    {
        _receiptStorage = receiptStorage;
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteReceiptCommand request, CancellationToken cancellationToken)
    {
        await _receiptStorage.DeleteAsync(request.ReceiptId, request.UserId, cancellationToken);
        if (Guid.TryParse(request.ReceiptId, out var receiptId))
        {
            var receipt = await _repository.GetByIdAndUserAsync(
                receiptId, request.UserId, cancellationToken);
            if (receipt != null)
            {
                receipt.SoftDelete();
                await _repository.SaveChangesAsync(cancellationToken);
            }
        }

        return Unit.Value;
    }
}
