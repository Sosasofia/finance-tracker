using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Services;

public class InstallmentService : IInstallmentService
{
    public List<Installment> GenerateInstallments(CreateTransactionDto transactionDto)
    {
        if (transactionDto.Installment == null || !transactionDto.IsCreditCardPurchase)
        {
            return new List<Installment>();
        }

        var installments = new List<Installment>();
        var installmentAmount = Math.Round(transactionDto.Amount / transactionDto.Installment.Number, 2);


        for (int i = 1; i <= transactionDto.Installment.Number; i++)
        {
            installments.Add(new Installment
            {
                Amount = installmentAmount,
                InstallmentNumber = i,
                DueDate = transactionDto.Date.AddMonths(i),
                IsPaid = false,
            });
        }

        return installments;
    }
}
