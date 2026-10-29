using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.DeletePaymentMethod;

public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand, Unit>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public DeletePaymentMethodCommandHandler(IPaymentMethodRepository repository)
    {
        _paymentMethodRepository = repository;
    }

    public async Task<Unit> Handle(DeletePaymentMethodCommand command, CancellationToken ct)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.Id, command.UserId, ct)
            ?? throw new NotFoundException("Payment Method", command.Id);

        if (paymentMethod.UserId != command.UserId)
        {
            throw new ForbiddenAccessException("System payment methods cannot be deleted.");
        }

        var inUse = await _paymentMethodRepository.IsInUseAsync(command.Id, ct);
        if (inUse)
        {
            throw new InUseException(command.Id, "payment method");
        }

        await _paymentMethodRepository.DeleteAsync(paymentMethod, ct);

        return Unit.Value;
    }
}
