using FinanceTracker.Application.Features.Transactions;
using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IInstallmentService
{
    /// <summary>
    /// Generates a list of installments for a credit card transaction based on the provided transaction details.
    /// </summary>
    /// <remarks>Each installment will have an equal amount, rounded to two decimal places, and a due date
    /// calculated  by adding the installment number in months to the transaction date. All installments are initially
    /// marked as unpaid.</remarks>
    /// <param name="transactionDto">The transaction details, including the total amount, installment information, and purchase type.</param>
    /// <returns>A list of <see cref="Installment"/> objects representing the generated installments.  Returns an empty list if
    /// the transaction does not include installment information or is not a credit card purchase.</returns>
    List<Installment> GenerateInstallments(CreateTransactionDto transactionDto);
}
