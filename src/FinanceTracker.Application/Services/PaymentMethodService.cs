using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Categories;
using FinanceTracker.Application.Features.PaymentMethods;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync()
    {
        var paymentMethods = await _paymentMethodRepository.GetPaymentMethods();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(paymentMethods);
    }

    public async Task<PaymentMethodDto> GetByIdAsync(Guid paymentMethodId)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodId)
            ?? throw new NotFoundException(nameof(PaymentMethod), paymentMethodId);

        return _mapper.Map<PaymentMethodDto>(paymentMethod);
    }

    public async Task<PaymentMethodDto> CreateAsync(Guid userId, CreatePaymentMethodDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Type))
        {
            throw new InvalidOperationException("Invalid payment method data.");
        }

        var exists = await _paymentMethodRepository.ExistsForUserAsync(userId, dto.Name);

        if (exists)
        {
            throw new DuplicateException("A payment method with this name already exists for this user.");
        }

        var newPaymentMethodEntity = _mapper.Map<PaymentMethod>(dto);

        newPaymentMethodEntity.UserId = userId;

        var createdPaymentMethod = await _paymentMethodRepository.AddAsync(newPaymentMethodEntity);

        return _mapper.Map<PaymentMethodDto>(createdPaymentMethod);
    }

    public async Task DeleteAsync(Guid paymentMethodId)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(paymentMethodId) ?? throw new NotFoundException(nameof(paymentMethodId), paymentMethodId);

        var inUse = await _paymentMethodRepository.IsInUseAsync(paymentMethodId);

        if (inUse)
        {
            throw new InvalidOperationException("Cannot delete payment method because it is referenced by transactions.");
        }

        await _paymentMethodRepository.DeleteAsync(paymentMethod);
    }
}
