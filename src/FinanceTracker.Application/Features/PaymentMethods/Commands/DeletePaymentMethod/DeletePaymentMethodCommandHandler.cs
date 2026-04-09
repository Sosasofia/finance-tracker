using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.DeletePaymentMethod;

public class DeletePaymentMethodCommandHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public DeletePaymentMethodCommandHandler(IPaymentMethodRepository repository)
    {
        _paymentMethodRepository = repository;
    }

    public async Task Handle(DeletePaymentMethodCommand command, CancellationToken ct)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.Id)
            ?? throw new NotFoundException("Payment Method", command.Id);

        if (paymentMethod.UserId != command.UserId)
            throw new ForbiddenAccessException("You cannot delete a payment method you do not own.");

        var inUse = await _paymentMethodRepository.IsInUseAsync(command.Id);
        if (inUse)
            throw new InvalidOperationException("Cannot delete payment method as it is in use by transactions.");

        await _paymentMethodRepository.DeleteAsync(paymentMethod);
    }
}
