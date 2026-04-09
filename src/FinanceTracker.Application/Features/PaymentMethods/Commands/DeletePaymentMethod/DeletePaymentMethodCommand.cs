namespace FinanceTracker.Application.Features.PaymentMethods.Commands.DeletePaymentMethod;

public record DeletePaymentMethodCommand(Guid Id, Guid UserId);
