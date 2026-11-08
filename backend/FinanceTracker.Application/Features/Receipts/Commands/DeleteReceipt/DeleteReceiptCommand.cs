using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands.DeleteReceipt;

public sealed record DeleteReceiptCommand(string ReceiptId, Guid UserId) : IRequest<Unit>;
