using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;

public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, TransactionResponse>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public CreateTransactionCommandHandler(
        ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository,
        IPaymentMethodRepository paymentMethodRepository)
    {
        _transactionRepository = transactionRepository;
        _categoryRepository = categoryRepository;
        _paymentMethodRepository = paymentMethodRepository;
    }

    public async Task<TransactionResponse> Handle(CreateTransactionCommand command, CancellationToken ct)
    {
        var category = await _categoryRepository.GetByIdAsync(command.CategoryId, command.UserId, ct)
            ?? throw new NotFoundException("Category", command.CategoryId);

        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.PaymentMethodId, command.UserId, ct)
            ?? throw new NotFoundException("Payment Method", command.PaymentMethodId);

        var amount = Money.Create(command.Amount);
        var utcDate = command.Date.Kind == DateTimeKind.Utc
            ? command.Date
            : command.Date.ToUniversalTime();

        var transaction = Transaction.Create(
            amount,
            command.Name,
            utcDate,
            command.Type,
            command.UserId,
            command.CategoryId,
            command.PaymentMethodId
        );

        if (!string.IsNullOrWhiteSpace(command.Description))
        {
            transaction.UpdateDetails(transaction.Name, command.Description);
        }

        if (!string.IsNullOrWhiteSpace(command.Notes))
        {
            transaction.UpdateNotes(command.Notes);
        }

        if (!string.IsNullOrWhiteSpace(command.ReceiptUrl))
        {
            transaction.AttachReceipt(command.ReceiptUrl);
        }

        if (command.IsCreditCardPurchase && command.Installment != null)
        {
            transaction.GenerateInstallments(command.Installment.Number);
        }

        if (command.IsReimbursement && command.Reimbursement != null)
        {
            var rAmount = Money.Create(command.Reimbursement.Amount);
            transaction.AddReimbursement(rAmount, command.Reimbursement.Reason ?? "No reason provided");
        }

        await _transactionRepository.AddTransactionAsync(transaction, ct);

        return TransactionResponse.MapFrom(transaction);
    }
}
