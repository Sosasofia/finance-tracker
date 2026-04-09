using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.CreatePaymentMethod;

public class CreatePaymentMethodCommandHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;

    public CreatePaymentMethodCommandHandler(IPaymentMethodRepository repository, IMapper mapper)
    {
        _paymentMethodRepository = repository;
        _mapper = mapper;
    }

    public async Task<PaymentMethodDto> Handle(CreatePaymentMethodCommand command, CancellationToken ct)
    {
        var exists = await _paymentMethodRepository.ExistsForUserAsync(command.UserId, command.Name);
        if (exists) throw new DuplicateException("A payment method with this name already exists.");

        var paymentMethod = PaymentMethod.Create(command.Name, command.Type, command.UserId);

        var created = await _paymentMethodRepository.AddAsync(paymentMethod);
        return _mapper.Map<PaymentMethodDto>(created);
    }
}
