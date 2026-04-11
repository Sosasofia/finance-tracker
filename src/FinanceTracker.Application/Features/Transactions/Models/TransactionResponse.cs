using FinanceTracker.Application.Features.Categories.Models;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Features.Transactions.Models;

public record TransactionResponse
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "ARS";
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime Date { get; init; }
    public string? Notes { get; init; }
    public string? ReceiptUrl { get; init; }
    public TransactionType Type { get; init; }
    public bool IsCreditCardPurchase { get; init; }
    public Guid UserId { get; init; }
    public Guid? CategoryId { get; init; }
    public CategoryDto? Category { get; init; }
    public Guid? PaymentMethodId { get; init; }
    public PaymentMethodDto? PaymentMethod { get; init; }
    public IEnumerable<InstallmentResponse> Installments { get; init; } = [];
    public ReimbursementDto? Reimbursement { get; init; }

    public static TransactionResponse MapFrom(Transaction src) => new()
    {
        Id = src.Id,
        Amount = src.Money.Amount,
        Currency = src.Money.Currency,
        Name = src.Name,
        Description = src.Description,
        Date = src.Date,
        Notes = src.Notes,
        ReceiptUrl = src.ReceiptUrl,
        Type = src.Type,
        IsCreditCardPurchase = src.IsCreditCardPurchase,
        UserId = src.UserId,

        CategoryId = src.CategoryId,
        PaymentMethodId = src.PaymentMethodId,

        Category = src.Category != null ? CategoryDto.MapFrom(src.Category) : null,
        PaymentMethod = src.PaymentMethod != null ? PaymentMethodDto.MapFrom(src.PaymentMethod) : null,

        Installments = src.Installments.Select(i => new InstallmentResponse(
            i.Id,
            i.InstallmentNumber,
            i.Money.Amount,
            i.DueDate,
            i.IsPaid,
            i.Money.Currency,
            i.PaymentDate)),

        Reimbursement = src.Reimbursement != null ? new ReimbursementDto(
            src.Reimbursement.Money.Amount,
            src.Reimbursement.Date,
            src.Reimbursement.Reason) : null
    };
}
