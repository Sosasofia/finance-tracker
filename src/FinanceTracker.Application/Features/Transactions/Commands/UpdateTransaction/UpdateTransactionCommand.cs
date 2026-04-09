namespace FinanceTracker.Application.Features.Transactions.Commands.UpdateTransaction;

public record UpdateTransactionCommand
{
    public Guid Id { get; init; }
    public Guid UserId { get; set; }

    public required string Name { get; init; }
    public string? Description { get; init; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public Guid CategoryId { get; init; }
    public Guid PaymentMethodId { get; init; }
}
