using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.DeletePaymentMethod;

public class DeletePaymentMethodCommandHandler : IRequestHandler<DeletePaymentMethodCommand>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public DeletePaymentMethodCommandHandler(IPaymentMethodRepository repository)
    {
        _paymentMethodRepository = repository;
    }

    public async Task Handle(DeletePaymentMethodCommand command, CancellationToken ct)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(command.Id, command.UserId, ct)
            ?? throw new NotFoundException("Payment Method", command.Id);

        if (paymentMethod.UserId != command.UserId)
            throw new ForbiddenAccessException("System payment methods cannot be deleted.");

        var inUse = await _paymentMethodRepository.IsInUseAsync(command.Id, ct);
        if (inUse)
            throw new InUseException(command.Id, "payment method");

        await _paymentMethodRepository.DeleteAsync(paymentMethod, ct);
    }
}
